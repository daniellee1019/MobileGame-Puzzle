extern "C" void RegisterStaticallyLinkedModulesGranular()
{
	void RegisterModule_SharedInternals();
	RegisterModule_SharedInternals();

	void RegisterModule_Core();
	RegisterModule_Core();

	void RegisterModule_AI();
	RegisterModule_AI();

	void RegisterModule_AndroidJNI();
	RegisterModule_AndroidJNI();

	void RegisterModule_Animation();
	RegisterModule_Animation();

	void RegisterModule_Audio();
	RegisterModule_Audio();

	void RegisterModule_Director();
	RegisterModule_Director();

	void RegisterModule_IMGUI();
	RegisterModule_IMGUI();

	void RegisterModule_Input();
	RegisterModule_Input();

	void RegisterModule_InputLegacy();
	RegisterModule_InputLegacy();

	void RegisterModule_JSONSerialize();
	RegisterModule_JSONSerialize();

	void RegisterModule_ParticleSystem();
	RegisterModule_ParticleSystem();

	void RegisterModule_Physics();
	RegisterModule_Physics();

	void RegisterModule_Physics2D();
	RegisterModule_Physics2D();

	void RegisterModule_RuntimeInitializeOnLoadManagerInitializer();
	RegisterModule_RuntimeInitializeOnLoadManagerInitializer();

	void RegisterModule_SpriteMask();
	RegisterModule_SpriteMask();

	void RegisterModule_Subsystems();
	RegisterModule_Subsystems();

	void RegisterModule_TextRendering();
	RegisterModule_TextRendering();

	void RegisterModule_TextCoreFontEngine();
	RegisterModule_TextCoreFontEngine();

	void RegisterModule_TextCoreTextEngine();
	RegisterModule_TextCoreTextEngine();

	void RegisterModule_UI();
	RegisterModule_UI();

	void RegisterModule_UIElements();
	RegisterModule_UIElements();

	void RegisterModule_VFX();
	RegisterModule_VFX();

	void RegisterModule_XR();
	RegisterModule_XR();

	void RegisterModule_VR();
	RegisterModule_VR();

}

template <typename T> void RegisterUnityClass(const char*);
template <typename T> void RegisterStrippedType(int, const char*, const char*);

void InvokeRegisterStaticallyLinkedModuleClasses()
{
	// Do nothing (we're in stripping mode)
}

class NavMeshAgent; template <> void RegisterUnityClass<NavMeshAgent>(const char*);
class NavMeshData; template <> void RegisterUnityClass<NavMeshData>(const char*);
class NavMeshProjectSettings; template <> void RegisterUnityClass<NavMeshProjectSettings>(const char*);
class NavMeshSettings; template <> void RegisterUnityClass<NavMeshSettings>(const char*);
class Animation; template <> void RegisterUnityClass<Animation>(const char*);
class AnimationClip; template <> void RegisterUnityClass<AnimationClip>(const char*);
class Animator; template <> void RegisterUnityClass<Animator>(const char*);
class AnimatorController; template <> void RegisterUnityClass<AnimatorController>(const char*);
class AnimatorOverrideController; template <> void RegisterUnityClass<AnimatorOverrideController>(const char*);
class Avatar; template <> void RegisterUnityClass<Avatar>(const char*);
class AvatarMask; template <> void RegisterUnityClass<AvatarMask>(const char*);
class Motion; template <> void RegisterUnityClass<Motion>(const char*);
class RuntimeAnimatorController; template <> void RegisterUnityClass<RuntimeAnimatorController>(const char*);
class AudioBehaviour; template <> void RegisterUnityClass<AudioBehaviour>(const char*);
class AudioClip; template <> void RegisterUnityClass<AudioClip>(const char*);
class AudioListener; template <> void RegisterUnityClass<AudioListener>(const char*);
class AudioManager; template <> void RegisterUnityClass<AudioManager>(const char*);
class AudioSource; template <> void RegisterUnityClass<AudioSource>(const char*);
class SampleClip; template <> void RegisterUnityClass<SampleClip>(const char*);
class Behaviour; template <> void RegisterUnityClass<Behaviour>(const char*);
class BuildSettings; template <> void RegisterUnityClass<BuildSettings>(const char*);
class Camera; template <> void RegisterUnityClass<Camera>(const char*);
namespace Unity { class Component; } template <> void RegisterUnityClass<Unity::Component>(const char*);
class ComputeShader; template <> void RegisterUnityClass<ComputeShader>(const char*);
class Cubemap; template <> void RegisterUnityClass<Cubemap>(const char*);
class CubemapArray; template <> void RegisterUnityClass<CubemapArray>(const char*);
class DelayedCallManager; template <> void RegisterUnityClass<DelayedCallManager>(const char*);
class EditorExtension; template <> void RegisterUnityClass<EditorExtension>(const char*);
class GameManager; template <> void RegisterUnityClass<GameManager>(const char*);
class GameObject; template <> void RegisterUnityClass<GameObject>(const char*);
class GlobalGameManager; template <> void RegisterUnityClass<GlobalGameManager>(const char*);
class GraphicsSettings; template <> void RegisterUnityClass<GraphicsSettings>(const char*);
class InputManager; template <> void RegisterUnityClass<InputManager>(const char*);
class LevelGameManager; template <> void RegisterUnityClass<LevelGameManager>(const char*);
class Light; template <> void RegisterUnityClass<Light>(const char*);
class LightProbes; template <> void RegisterUnityClass<LightProbes>(const char*);
class LightingSettings; template <> void RegisterUnityClass<LightingSettings>(const char*);
class LightmapSettings; template <> void RegisterUnityClass<LightmapSettings>(const char*);
class LineRenderer; template <> void RegisterUnityClass<LineRenderer>(const char*);
class LowerResBlitTexture; template <> void RegisterUnityClass<LowerResBlitTexture>(const char*);
class Material; template <> void RegisterUnityClass<Material>(const char*);
class Mesh; template <> void RegisterUnityClass<Mesh>(const char*);
class MeshFilter; template <> void RegisterUnityClass<MeshFilter>(const char*);
class MeshRenderer; template <> void RegisterUnityClass<MeshRenderer>(const char*);
class MonoBehaviour; template <> void RegisterUnityClass<MonoBehaviour>(const char*);
class MonoManager; template <> void RegisterUnityClass<MonoManager>(const char*);
class MonoScript; template <> void RegisterUnityClass<MonoScript>(const char*);
class NamedObject; template <> void RegisterUnityClass<NamedObject>(const char*);
class Object; template <> void RegisterUnityClass<Object>(const char*);
class PlayerSettings; template <> void RegisterUnityClass<PlayerSettings>(const char*);
class PreloadData; template <> void RegisterUnityClass<PreloadData>(const char*);
class QualitySettings; template <> void RegisterUnityClass<QualitySettings>(const char*);
namespace UI { class RectTransform; } template <> void RegisterUnityClass<UI::RectTransform>(const char*);
class ReflectionProbe; template <> void RegisterUnityClass<ReflectionProbe>(const char*);
class RenderSettings; template <> void RegisterUnityClass<RenderSettings>(const char*);
class RenderTexture; template <> void RegisterUnityClass<RenderTexture>(const char*);
class Renderer; template <> void RegisterUnityClass<Renderer>(const char*);
class ResourceManager; template <> void RegisterUnityClass<ResourceManager>(const char*);
class RuntimeInitializeOnLoadManager; template <> void RegisterUnityClass<RuntimeInitializeOnLoadManager>(const char*);
class Shader; template <> void RegisterUnityClass<Shader>(const char*);
class ShaderNameRegistry; template <> void RegisterUnityClass<ShaderNameRegistry>(const char*);
class Skybox; template <> void RegisterUnityClass<Skybox>(const char*);
class SortingGroup; template <> void RegisterUnityClass<SortingGroup>(const char*);
class Sprite; template <> void RegisterUnityClass<Sprite>(const char*);
class SpriteAtlas; template <> void RegisterUnityClass<SpriteAtlas>(const char*);
class SpriteRenderer; template <> void RegisterUnityClass<SpriteRenderer>(const char*);
class TagManager; template <> void RegisterUnityClass<TagManager>(const char*);
class TextAsset; template <> void RegisterUnityClass<TextAsset>(const char*);
class Texture; template <> void RegisterUnityClass<Texture>(const char*);
class Texture2D; template <> void RegisterUnityClass<Texture2D>(const char*);
class Texture2DArray; template <> void RegisterUnityClass<Texture2DArray>(const char*);
class Texture3D; template <> void RegisterUnityClass<Texture3D>(const char*);
class TimeManager; template <> void RegisterUnityClass<TimeManager>(const char*);
class Transform; template <> void RegisterUnityClass<Transform>(const char*);
class PlayableDirector; template <> void RegisterUnityClass<PlayableDirector>(const char*);
class ParticleSystem; template <> void RegisterUnityClass<ParticleSystem>(const char*);
class ParticleSystemRenderer; template <> void RegisterUnityClass<ParticleSystemRenderer>(const char*);
class BoxCollider; template <> void RegisterUnityClass<BoxCollider>(const char*);
class CapsuleCollider; template <> void RegisterUnityClass<CapsuleCollider>(const char*);
class Collider; template <> void RegisterUnityClass<Collider>(const char*);
class MeshCollider; template <> void RegisterUnityClass<MeshCollider>(const char*);
class PhysicsManager; template <> void RegisterUnityClass<PhysicsManager>(const char*);
class Rigidbody; template <> void RegisterUnityClass<Rigidbody>(const char*);
class SphereCollider; template <> void RegisterUnityClass<SphereCollider>(const char*);
class Collider2D; template <> void RegisterUnityClass<Collider2D>(const char*);
class CompositeCollider2D; template <> void RegisterUnityClass<CompositeCollider2D>(const char*);
class Physics2DSettings; template <> void RegisterUnityClass<Physics2DSettings>(const char*);
class PolygonCollider2D; template <> void RegisterUnityClass<PolygonCollider2D>(const char*);
class Rigidbody2D; template <> void RegisterUnityClass<Rigidbody2D>(const char*);
namespace TextRendering { class Font; } template <> void RegisterUnityClass<TextRendering::Font>(const char*);
namespace TextRenderingPrivate { class TextMesh; } template <> void RegisterUnityClass<TextRenderingPrivate::TextMesh>(const char*);
namespace UI { class Canvas; } template <> void RegisterUnityClass<UI::Canvas>(const char*);
namespace UI { class CanvasGroup; } template <> void RegisterUnityClass<UI::CanvasGroup>(const char*);
namespace UI { class CanvasRenderer; } template <> void RegisterUnityClass<UI::CanvasRenderer>(const char*);
class VFXManager; template <> void RegisterUnityClass<VFXManager>(const char*);
class VisualEffect; template <> void RegisterUnityClass<VisualEffect>(const char*);
class VisualEffectAsset; template <> void RegisterUnityClass<VisualEffectAsset>(const char*);
class VisualEffectObject; template <> void RegisterUnityClass<VisualEffectObject>(const char*);

void RegisterAllClasses()
{
void RegisterBuiltinTypes();
RegisterBuiltinTypes();
	//Total: 98 non stripped classes
	//0. NavMeshAgent
	RegisterUnityClass<NavMeshAgent>("AI");
	//1. NavMeshData
	RegisterUnityClass<NavMeshData>("AI");
	//2. NavMeshProjectSettings
	RegisterUnityClass<NavMeshProjectSettings>("AI");
	//3. NavMeshSettings
	RegisterUnityClass<NavMeshSettings>("AI");
	//4. Animation
	RegisterUnityClass<Animation>("Animation");
	//5. AnimationClip
	RegisterUnityClass<AnimationClip>("Animation");
	//6. Animator
	RegisterUnityClass<Animator>("Animation");
	//7. AnimatorController
	RegisterUnityClass<AnimatorController>("Animation");
	//8. AnimatorOverrideController
	RegisterUnityClass<AnimatorOverrideController>("Animation");
	//9. Avatar
	RegisterUnityClass<Avatar>("Animation");
	//10. AvatarMask
	RegisterUnityClass<AvatarMask>("Animation");
	//11. Motion
	RegisterUnityClass<Motion>("Animation");
	//12. RuntimeAnimatorController
	RegisterUnityClass<RuntimeAnimatorController>("Animation");
	//13. AudioBehaviour
	RegisterUnityClass<AudioBehaviour>("Audio");
	//14. AudioClip
	RegisterUnityClass<AudioClip>("Audio");
	//15. AudioListener
	RegisterUnityClass<AudioListener>("Audio");
	//16. AudioManager
	RegisterUnityClass<AudioManager>("Audio");
	//17. AudioSource
	RegisterUnityClass<AudioSource>("Audio");
	//18. SampleClip
	RegisterUnityClass<SampleClip>("Audio");
	//19. Behaviour
	RegisterUnityClass<Behaviour>("Core");
	//20. BuildSettings
	RegisterUnityClass<BuildSettings>("Core");
	//21. Camera
	RegisterUnityClass<Camera>("Core");
	//22. Component
	RegisterUnityClass<Unity::Component>("Core");
	//23. ComputeShader
	RegisterUnityClass<ComputeShader>("Core");
	//24. Cubemap
	RegisterUnityClass<Cubemap>("Core");
	//25. CubemapArray
	RegisterUnityClass<CubemapArray>("Core");
	//26. DelayedCallManager
	RegisterUnityClass<DelayedCallManager>("Core");
	//27. EditorExtension
	RegisterUnityClass<EditorExtension>("Core");
	//28. GameManager
	RegisterUnityClass<GameManager>("Core");
	//29. GameObject
	RegisterUnityClass<GameObject>("Core");
	//30. GlobalGameManager
	RegisterUnityClass<GlobalGameManager>("Core");
	//31. GraphicsSettings
	RegisterUnityClass<GraphicsSettings>("Core");
	//32. InputManager
	RegisterUnityClass<InputManager>("Core");
	//33. LevelGameManager
	RegisterUnityClass<LevelGameManager>("Core");
	//34. Light
	RegisterUnityClass<Light>("Core");
	//35. LightProbes
	RegisterUnityClass<LightProbes>("Core");
	//36. LightingSettings
	RegisterUnityClass<LightingSettings>("Core");
	//37. LightmapSettings
	RegisterUnityClass<LightmapSettings>("Core");
	//38. LineRenderer
	RegisterUnityClass<LineRenderer>("Core");
	//39. LowerResBlitTexture
	RegisterUnityClass<LowerResBlitTexture>("Core");
	//40. Material
	RegisterUnityClass<Material>("Core");
	//41. Mesh
	RegisterUnityClass<Mesh>("Core");
	//42. MeshFilter
	RegisterUnityClass<MeshFilter>("Core");
	//43. MeshRenderer
	RegisterUnityClass<MeshRenderer>("Core");
	//44. MonoBehaviour
	RegisterUnityClass<MonoBehaviour>("Core");
	//45. MonoManager
	RegisterUnityClass<MonoManager>("Core");
	//46. MonoScript
	RegisterUnityClass<MonoScript>("Core");
	//47. NamedObject
	RegisterUnityClass<NamedObject>("Core");
	//48. Object
	//Skipping Object
	//49. PlayerSettings
	RegisterUnityClass<PlayerSettings>("Core");
	//50. PreloadData
	RegisterUnityClass<PreloadData>("Core");
	//51. QualitySettings
	RegisterUnityClass<QualitySettings>("Core");
	//52. RectTransform
	RegisterUnityClass<UI::RectTransform>("Core");
	//53. ReflectionProbe
	RegisterUnityClass<ReflectionProbe>("Core");
	//54. RenderSettings
	RegisterUnityClass<RenderSettings>("Core");
	//55. RenderTexture
	RegisterUnityClass<RenderTexture>("Core");
	//56. Renderer
	RegisterUnityClass<Renderer>("Core");
	//57. ResourceManager
	RegisterUnityClass<ResourceManager>("Core");
	//58. RuntimeInitializeOnLoadManager
	RegisterUnityClass<RuntimeInitializeOnLoadManager>("Core");
	//59. Shader
	RegisterUnityClass<Shader>("Core");
	//60. ShaderNameRegistry
	RegisterUnityClass<ShaderNameRegistry>("Core");
	//61. Skybox
	RegisterUnityClass<Skybox>("Core");
	//62. SortingGroup
	RegisterUnityClass<SortingGroup>("Core");
	//63. Sprite
	RegisterUnityClass<Sprite>("Core");
	//64. SpriteAtlas
	RegisterUnityClass<SpriteAtlas>("Core");
	//65. SpriteRenderer
	RegisterUnityClass<SpriteRenderer>("Core");
	//66. TagManager
	RegisterUnityClass<TagManager>("Core");
	//67. TextAsset
	RegisterUnityClass<TextAsset>("Core");
	//68. Texture
	RegisterUnityClass<Texture>("Core");
	//69. Texture2D
	RegisterUnityClass<Texture2D>("Core");
	//70. Texture2DArray
	RegisterUnityClass<Texture2DArray>("Core");
	//71. Texture3D
	RegisterUnityClass<Texture3D>("Core");
	//72. TimeManager
	RegisterUnityClass<TimeManager>("Core");
	//73. Transform
	RegisterUnityClass<Transform>("Core");
	//74. PlayableDirector
	RegisterUnityClass<PlayableDirector>("Director");
	//75. ParticleSystem
	RegisterUnityClass<ParticleSystem>("ParticleSystem");
	//76. ParticleSystemRenderer
	RegisterUnityClass<ParticleSystemRenderer>("ParticleSystem");
	//77. BoxCollider
	RegisterUnityClass<BoxCollider>("Physics");
	//78. CapsuleCollider
	RegisterUnityClass<CapsuleCollider>("Physics");
	//79. Collider
	RegisterUnityClass<Collider>("Physics");
	//80. MeshCollider
	RegisterUnityClass<MeshCollider>("Physics");
	//81. PhysicsManager
	RegisterUnityClass<PhysicsManager>("Physics");
	//82. Rigidbody
	RegisterUnityClass<Rigidbody>("Physics");
	//83. SphereCollider
	RegisterUnityClass<SphereCollider>("Physics");
	//84. Collider2D
	RegisterUnityClass<Collider2D>("Physics2D");
	//85. CompositeCollider2D
	RegisterUnityClass<CompositeCollider2D>("Physics2D");
	//86. Physics2DSettings
	RegisterUnityClass<Physics2DSettings>("Physics2D");
	//87. PolygonCollider2D
	RegisterUnityClass<PolygonCollider2D>("Physics2D");
	//88. Rigidbody2D
	RegisterUnityClass<Rigidbody2D>("Physics2D");
	//89. Font
	RegisterUnityClass<TextRendering::Font>("TextRendering");
	//90. TextMesh
	RegisterUnityClass<TextRenderingPrivate::TextMesh>("TextRendering");
	//91. Canvas
	RegisterUnityClass<UI::Canvas>("UI");
	//92. CanvasGroup
	RegisterUnityClass<UI::CanvasGroup>("UI");
	//93. CanvasRenderer
	RegisterUnityClass<UI::CanvasRenderer>("UI");
	//94. VFXManager
	RegisterUnityClass<VFXManager>("VFX");
	//95. VisualEffect
	RegisterUnityClass<VisualEffect>("VFX");
	//96. VisualEffectAsset
	RegisterUnityClass<VisualEffectAsset>("VFX");
	//97. VisualEffectObject
	RegisterUnityClass<VisualEffectObject>("VFX");

}
