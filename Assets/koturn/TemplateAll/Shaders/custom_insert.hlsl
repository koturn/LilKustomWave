// _AudioTexture is declared in lil_common_input.hlsl
// when LIL_FEATURE_AUDIOLINK is defined.
// #ifndef LIL_FEATURE_AUDIOLINK
// TEXTURE2D_FLOAT(_AudioTexture);
// float4 _AudioTexture_TexelSize;
// #endif  // LIL_FEATURE_AUDIOLINK

// 0: Rendering normally
// 1: Rendering in VR handheld camera
// 2: Rendering in Desktop handheld camera
// 3: Rendering for a screenshot
// float _VRChatCameraMode

// 0: Rendering normally, not in a mirror
// 1: Rendering in a mirror viewed in VR
// 2: Rendering in a mirror viewed in desktop mode
// float _VRChatMirrorMode

// World space position of mirror camera (eye independent, "centered" in VR)
// float _VRChatMirrorCameraPos
