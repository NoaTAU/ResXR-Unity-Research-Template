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

    [Header("References")]
    public Transform contentGridContainer;
    public GameObject textBlockPrefab;
    public GameObject Back;


    [Header("Settings")]
    [Range(1, 60)]
    public float UIrefreshRateHz = 15f;
    public int entriesPerBlock = 16;
    public bool expandOnStart = false;


    private LiveMonitorService _tap;
    private ColumnIndex _schema;
    private LiveColumnGroup _groupSpec;
    private List<TextMeshPro> _blocks = new List<TextMeshPro>();

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

        BuildBlocks();
    }

    private void BuildBlocks()
    {
        foreach (Transform child in contentGridContainer)
            Destroy(child.gameObject);
        _blocks.Clear();

        int total = _groupSpec.AllColumns.Count;
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

        int total = _groupSpec.AllColumns.Count;

        for (int blockIndex = 0; blockIndex < _blocks.Count; blockIndex++)
        {
            int start = blockIndex * entriesPerBlock;
            int end = Mathf.Min(start + entriesPerBlock, total);

            var sb = new StringBuilder(256);

            for (int i = start; i < end; i++)
            {
                int colIdx = _groupSpec.AllColumns[i];
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


