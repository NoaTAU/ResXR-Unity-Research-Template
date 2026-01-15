# CONTINUOUSDATA_V2 - column <- source

# - Timing (Unity clock kept for continuity) -
# [Collector: ResXRDataManager_V2 (direct)]
timeSinceStartup                          <- Unity: Time.realTimeSinceStartup

# ============================================================
# COORDINATE SPACE CONVERSION
# ============================================================
# NOTE: All OVRPlugin APIs return data in TRACKING SPACE (right-handed coordinates,
# relative to VR play area origin). This system automatically converts spatial data
# to UNITY WORLD SPACE (left-handed coordinates, Unity scene's global system) using
# TrackingSpaceConverter.ToWorldSpacePosition() and ToWorldSpaceRotation().
#
# This conversion:
#   1. Applies Z-axis flip (right-handed → left-handed)
#   2. Rotates by OVRCameraRig's rotation
#   3. Adds OVRCameraRig's position offset
#
# Velocities and angular velocities are rotated (steps 1-2) but NOT offset.
# Hand bones remain in hand-local space (relative to hand root).
# ============================================================

# - Legacy head pose (Euler) from Head node -
# [Collector: OVRNodesCollector]
Head_Position_x                            <- OVRPlugin.GetNodePose(node, step).Posef.Position.x  [converted to world space]
Head_Height                                <- ...Position.y  [converted to world space]
Head_Position_Z                            <- ...Position.z  [converted to world space]
Gaze_Pitch                                 <- Node.Head Posef.Orientation <- Euler.x (degrees)  [converted to world space]
Gaze_Yaw                                   <- ...Euler.y  [converted to world space]
Gaze_Roll                                  <- ...Euler.z  [converted to world space]
HeadNodeOrientationValid                   <- OVRPlugin.GetNodeOrientationValid(Node.Head)           (0/1)
HeadNodePositionValid                      <- OVRPlugin.GetNodePositionValid(Node.Head)              (0/1)
HeadNodeOrientationTracked                 <- OVRPlugin.GetNodeOrientationTracked(Node.Head)         (0/1)
HeadNodePositionTracked                    <- OVRPlugin.GetNodePositionTracked(Node.Head)            (0/1)
HeadNodeTime                               <- from GetNodePoseStateRaw(<Node>, step).Time

# - Legacy gaze hit point/raycast  -
# [Collector: OVREyesCollector]
FocusedObject                              <- ResXRPlayer.Instance.FocusedObject (GameObject name or "none")
EyeGazeHitPosition_X                       <- ResXRPlayer.Instance.EyeGazeHitPosition.x  [world space]
EyeGazeHitPosition_Y                       <- ResXRPlayer.Instance.EyeGazeHitPosition.y  [world space]
EyeGazeHitPosition_Z                       <- ResXRPlayer.Instance.EyeGazeHitPosition.z  [world space]

# - Eye gazes (dedicated API) -
# [Collector: OVREyesCollector]
RightEye_qx                                <- OVRPlugin.GetEyeGazesState(step, frameIndex, ref state).EyeGazes[(int)OVRPlugin.Eye.Right].Pose.Orientation.x  [converted to world space]
RightEye_qy                                <- ...EyeGazes[(int)OVRPlugin.Eye.Right].Pose.Orientation.y  [converted to world space]
RightEye_qz                                <- ...EyeGazes[(int)OVRPlugin.Eye.Right].Pose.Orientation.z  [converted to world space]
RightEye_qw                                <- ...EyeGazes[(int)OVRPlugin.Eye.Right].Pose.Orientation.w  [converted to world space]
LeftEye_qx                                 <- ...EyeGazes[(int)OVRPlugin.Eye.Left].Pose.Orientation.x  [converted to world space]
LeftEye_qy                                 <- ...EyeGazes[(int)OVRPlugin.Eye.Left].Pose.Orientation.y  [converted to world space]
LeftEye_qz                                 <- ...EyeGazes[(int)OVRPlugin.Eye.Left].Pose.Orientation.z  [converted to world space]
LeftEye_qw                                 <- ...EyeGazes[(int)OVRPlugin.Eye.Left].Pose.Orientation.w  [converted to world space]
LeftEye_IsValid                            <- state.EyeGazes[(int)OVRPlugin.Eye.Left].IsValid         (0/1)
LeftEye_Confidence                         <- state.EyeGazes[(int)OVRPlugin.Eye.Left].Confidence      (string)
RightEye_IsValid                           <- state.EyeGazes[(int)OVRPlugin.Eye.Right].IsValid        (0/1)
RightEye_Confidence                        <- state.EyeGazes[(int)OVRPlugin.Eye.Right].Confidence     (string)
Eyes_Time                                  <- state.Time   (double, seconds; shared timestamp for both eyes)

# - Recenter flags (system) -
# [Collector: RecenterCollector]
shouldRecenter                             <- OVRPlugin.shouldRecenter                                (0/1 if OVRPlugin.shouldRecenter available. else empty to indicate its not reading from OVRPlugin)
recenterEvent                              <- Derived: rising edge of shouldRecenter                  (1 only on the first frame where shouldRecenter changes 0->1; otherwise 0, empty when OVRPlugin.shouldRecenter isn't available)

# - Device nodes (for each <Node> in order: EyeLeft, EyeRight, EyeCenter, Head, HandLeft, HandRight, ControllerLeft, ControllerRight) -
# [Collector: OVRNodesCollector]
Node_<Node>_Present                        <- OVRPlugin.GetNodePresent(<Node>)                        (0/1)
Node_<Node>_px / _py / _pz                 <- GetNodePose(<Node>, step).Position.{x,y,z}  [converted to world space]
Node_<Node>_qx / _qy / _qz / _qw           <- ...Posef.Orientation.{x,y,z,w}  [converted to world space]
Node_<Node>_Vel_x / _Vel_y / _Vel_z        <- OVRPlugin.GetNodeVelocity(<Node>, step).{x,y,z}  [converted to world space]
Node_<Node>_AngVel_x/_AngVel_y/_AngVel_z   <- OVRPlugin.GetNodeAngularVelocity(<Node>, step).{x,y,z}  [converted to world space]
Node_<Node>_Valid_Position                 <- OVRPlugin.GetNodePositionValid(<Node>)                  (0/1)
Node_<Node>_Valid_Orientation              <- OVRPlugin.GetNodeOrientationValid(<Node>)               (0/1)
Node_<Node>_Tracked_Position               <- OVRPlugin.GetNodePositionTracked(<Node>)                (0/1)
Node_<Node>_Tracked_Orientation            <- OVRPlugin.GetNodeOrientationTracked(<Node>)             (0/1)
Node_<Node>_Time                           <- from GetNodePoseStateRaw(<Node>, step).Time

# - Hands (dedicated API; LEFT then RIGHT) -
# [Collector: OVRHandsCollector]
LeftHand_Status                            <- OVRPlugin.GetHandState(step, Hand.HandLeft).Status
LeftHand_Root_px/_py/_pz                   <- HandState.RootPose.Position.{x,y,z}  [converted to world space]
LeftHand_Root_qx/_qy/_qz/_qw               <- HandState.RootPose.Orientation.{x,y,z,w}  [converted to world space]
LeftHand_HandScale                         <- HandState.HandScale
LeftHand_HandConfidence                    <- HandState.HandConfidence
LeftHand_FingerConf_Thumb                  <- HandState.FingerConfidences[Thumb]
LeftHand_FingerConf_Index                  <- HandState.FingerConfidences[Index]
LeftHand_FingerConf_Middle                 <- HandState.FingerConfidences[Middle]
LeftHand_FingerConf_Ring                   <- HandState.FingerConfidences[Ring]
LeftHand_FingerConf_Pinky                  <- HandState.FingerConfidences[Pinky]
LeftHand_RequestedTS                       <- HandState.RequestedTimeStamp
LeftHand_SampleTS                          <- HandState.SampleTimeStamp

# Per-bone (columns named by SDK BoneId enum; order = SDK bone enum order)
Left_<BoneName>_x/_y/_z                    <- HandState.BonePositions[boneIndex].{x,y,z}  (e.g., Left_Hand_Thumb0_x or Left_XRHand_Thumb_Metacarpal_x)  [hand-local space]
Left_<BoneName>_qx/_qy/_qz/_qw             <- HandState.BoneRotations[boneIndex].{x,y,z,w}  [hand-local space]

RightHand_*                                <- Same fields as LeftHand (Hand.HandRight)

# - Body (BodyState4; frame + joints) -
# [Collector: OVRBodyCollector]
Body_Time                                   <- OVRPlugin.GetBodyState4(step,...).Time
Body_Confidence                             <- state.Confidence
Body_Fidelity                               <- state.Fidelity
Body_CalibrationStatus                      <- state.CalibrationStatus
Body_SkeletonChangedCount                   <- state.SkeletonChangedCount

# For each joint J in SDK BoneId enum order (names starting with "Body_"):
<Body_JointName>_px/_py/_pz                          <- state.JointLocations[jointIndex].Pose.Position.{x,y,z}  (e.g., Body_Root_px, Body_Hips_px)  [converted to world space]
<Body_JointName>_qx/_qy/_qz/_qw                      <- state.JointLocations[jointIndex].Pose.Orientation.{x,y,z,w}  [converted to world space]
<Body_JointName>_Flags                               <- state.JointLocations[jointIndex].LocationFlags  (bitfield)

# - Perf -
# [Collector: OVRPerformanceCollector]
AppMotionToPhotonLatency                    <- OVRPlugin.GetAppPerfStats().FrameStats[last].AppMotionToPhotonLatency (seconds, if available)

# - Custom transforms (experiment-specific; registered order) -
# [Collector: CustomTransformsCollector]
Custom_<Name>_px/_py/_pz                    <- Unity Transform.position.{x,y,z}  [world space]
Custom_<Name>_qx/_qy/_qz/_qw                <- Unity Transform.rotation.{x,y,z,w}  [world space]



# FACEEXPRESSIONS_V2 - column <- source

# - Timing -
# [Collector: ResXRDataManager_V2 (direct)]
timeSinceStartup                          <- Unity: Time.realTimeSinceStartup (renamed from "TimeFromStart") [legacy continuity]

# - Face state (dedicated API: FaceState2) -
# [Collector: OVRFaceCollector]
Face_Time                                 <- FaceState.Time         (sdk timestamp, seconds)
Face_Status                               <- FaceState.Status.IsValid (bool: true/false)

# - Expression weights (OVRPlugin.FaceExpression2 names, one per entry) -
# [Collector: OVRFaceCollector]
Brow_Lowerer_L ... Tongue_Retreat           <- FaceState.ExpressionWeights[i] (int 0..69, one per enum entry)

# - Region confidences (OVRPlugin.FaceRegionConfidence) -
# [Collector: OVRFaceCollector]
FaceRegionConfidence_Upper, FaceRegionConfidence_Lower <- FaceState.ExpressionWeightConfidences[Upper/Lower] as string


# Notes:
# - Indices map to the FaceExpression enum order in the MetaXR v78 SDK guide.
# - Total per frame = 70 weights + 2 confidences + status + time + timeSinceStartup.
#
# Coordinate Space Definitions:
# - [converted to world space]: Originally from OVRPlugin in tracking space, converted to Unity world space via TrackingSpaceConverter
# - [world space]: Native Unity world space data (Transform.position, raycast hits, etc.)
# - [hand-local space]: Relative to the hand's root (wrist). Each hand bone is relative to its root pose.