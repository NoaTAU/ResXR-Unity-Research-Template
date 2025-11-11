using System.Collections.Generic;
using System.Text;
using TMPro;
using TXRData;
using UnityEngine;

public class LiveColumnGroupPanel : MonoBehaviour
{
    [Header("Live Stream Source")]
    public LiveStreamKind liveStreamKind;
    public LiveColumnGroupKind liveColumnGroupKind;
    [Header("Sub-grouping")]
    public LiveColumnSubGroupKind subGroupKind = LiveColumnSubGroupKind.All;


    [Header("References")]
    public Transform contentGridContainer;
    public GameObject textBlockPrefab;
    public GameObject Back;
    public TextMeshPro TitleText;

    [Header("Settings")]
    [Range(1, 60)]
    public float UIrefreshRateHz = 15f;
    public int entriesPerBlock = 16;
    public bool expandOnStart = false;


    private LiveMonitorService _tap;
    private ColumnIndex _schema;
    private LiveColumnGroup _groupSpec;
    private List<TextMeshPro> _blocks = new List<TextMeshPro>();
    private List<int> _displayColumns = new List<int>();


    private float _nextRefreshTime;

    private void Start()
    {
        _tap = LiveMonitorService.Instance;
        if (_tap == null)
        {
            Debug.LogWarning("[LiveColumnGroupPanel] LiveMonitorService not found.");
            enabled = false;
            return;
        }
        ExpandPanel(expandOnStart);

        StartCoroutine(WaitForSchemaAndBuild());
    }

    private System.Collections.IEnumerator WaitForSchemaAndBuild()
    {
        while (_schema == null)
        {
            LiveRow row;
            bool ok = liveStreamKind == LiveStreamKind.Continuous
                ? _tap.TryGetLatestContinuous(out row)
                : _tap.TryGetLatestFace(out row);

            if (ok && row.IsValid)
            {
                _schema = row.schema;
                break;
            }
            yield return null;
        }

        if (_schema == null)
        {
            Debug.LogWarning("[LiveColumnGroupPanel] No schema available.");
            yield break;
        }

        // Build groups, pick our group
        var groups = LiveColumnGroups.BuildGroups(_schema, liveStreamKind);
        _groupSpec = groups.Find(g => g.Kind == liveColumnGroupKind);
        if (_groupSpec == null || _groupSpec.AllColumns.Count == 0)
        {
            Debug.LogWarning($"[LiveColumnGroupPanel] No columns for group {liveColumnGroupKind}.");
            yield break;
        }

        // Build the per-panel column list based on sub-group kind
        _displayColumns = BuildDisplayColumnList();
        if (_displayColumns == null || _displayColumns.Count == 0)
        {
            Debug.LogWarning($"[LiveColumnGroupPanel] No display columns after filtering for {subGroupKind} in group {liveColumnGroupKind}. Falling back to All.");
            _displayColumns = new List<int>(_groupSpec.AllColumns);
        }

        BuildBlocks();
        // --- Auto title update ---
        if (TitleText != null)
        {
            TitleText.text = $"({liveStreamKind}) {liveColumnGroupKind} / {subGroupKind}";
        }

    }

    private List<int> BuildDisplayColumnList()
    {
        // Base list
        var source = _groupSpec.AllColumns;

        if (subGroupKind == LiveColumnSubGroupKind.All)
            return new List<int>(source);

        List<int> result = new List<int>();

        for (int i = 0; i < source.Count; i++)
        {
            int colIdx = source[i];
            string colName = _schema[colIdx];

            bool isFlag = IsFlagColumnName(colName);
            bool isTime = IsTimeColumnName(colName);

            switch (subGroupKind)
            {
                case LiveColumnSubGroupKind.FlagsOnly:
                    if (isFlag)
                        result.Add(colIdx);
                    break;

                case LiveColumnSubGroupKind.ValuesOnly:
                    // values = not flags, not time
                    if (!isFlag && !isTime)
                        result.Add(colIdx);
                    break;

                case LiveColumnSubGroupKind.TimeOnly:
                    if (isTime)
                        result.Add(colIdx);
                    break;
            }
        }

        return result;
    }


    private static bool IsFlagColumnName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        // Explicit singletons
        if (name == "shouldRecenter" || name == "recenterEvent")
            return true;

        // Suffix-based / pattern-based flags
        if (name.EndsWith("_Flags", System.StringComparison.Ordinal))
            return true;
        if (name.EndsWith("_IsValid", System.StringComparison.Ordinal))
            return true;

        // Generic states
        if (name.Contains("Status", System.StringComparison.Ordinal))
            return true;
        if (name.Contains("Present", System.StringComparison.Ordinal))
            return true;
        if (name.Contains("Valid_", System.StringComparison.Ordinal))
            return true;
        if (name.Contains("Tracked_", System.StringComparison.Ordinal))
            return true;
        if (name.Contains("Conf", System.StringComparison.Ordinal))
            return true;

        // You can extend this with more heuristics if you want
        return false;
    }

    private static bool IsTimeColumnName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        // Explicitly NOT time: you asked to keep this in "other"
        if (name == "timeSinceStartup")
            return false;

        // Any column with "Time" in the name:
        // Node_*_Time, Eyes_Time, Body_Time, Face_Time, HeadNodeTime, etc.
        if (name.Contains("Time", System.StringComparison.Ordinal))
            return true;

        // Hand timestamps: RequestedTS / SampleTS
        if (name.EndsWith("TS", System.StringComparison.Ordinal) ||
            name.Contains("_TS", System.StringComparison.Ordinal))
            return true;

        return false;
    }



    private void BuildBlocks()
    {
        foreach (Transform child in contentGridContainer)
            Destroy(child.gameObject);
        _blocks.Clear();

        int total = _displayColumns.Count;

        int blockCount = Mathf.Max(1, (total + entriesPerBlock - 1) / entriesPerBlock);

        for (int i = 0; i < blockCount; i++)
        {
            TextMeshPro inst = Instantiate(textBlockPrefab, contentGridContainer).GetComponent<TextMeshPro>();
            inst.text = "";
            _blocks.Add(inst);
        }
    }

    private void Update()
    {
        if (_schema == null || _blocks.Count == 0) return;

        float now = Time.unscaledTime;
        if (now < _nextRefreshTime) return;
        _nextRefreshTime = now + (1f / Mathf.Max(1f, UIrefreshRateHz));

        LiveRow row;
        bool ok = liveStreamKind == LiveStreamKind.Continuous
            ? _tap.TryGetLatestContinuous(out row)
            : _tap.TryGetLatestFace(out row);

        if (!ok || !row.IsValid)
        {
            foreach (var b in _blocks) b.text = "(no data yet)";
            return;
        }

        UpdateBlocks(row);
    }

    private void UpdateBlocks(LiveRow row)
    {
        var schema = row.schema;
        var values = row.values;
        var mask = row.columnIsSetMask;

        int total = _displayColumns.Count;

        for (int blockIndex = 0; blockIndex < _blocks.Count; blockIndex++)
        {
            int start = blockIndex * entriesPerBlock;
            int end = Mathf.Min(start + entriesPerBlock, total);

            var sb = new StringBuilder(256);

            for (int i = start; i < end; i++)
            {
                int colIdx = _displayColumns[i];
                string name = schema[colIdx];

                sb.Append(name);
                sb.Append(": ");

                if (mask[colIdx] && values[colIdx] != null)
                    sb.Append(FormatValue(values[colIdx]));
                else
                    sb.Append("∅");

                if (i < end - 1)
                    sb.AppendLine();
            }

            _blocks[blockIndex].text = sb.ToString();
        }
    }

    private static string FormatValue(object value)
    {
        if (value == null) return "null";
        switch (value)
        {
            case float f: return f.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
            case double d: return d.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
            case int i: return i.ToString(System.Globalization.CultureInfo.InvariantCulture);
            case bool b: return b ? "true" : "false";
            default: return value.ToString();
        }
    }

    public void ExpandPanel(bool expand)
    {
        contentGridContainer.gameObject.SetActive(expand);
        Back.SetActive(expand);
    }
}


