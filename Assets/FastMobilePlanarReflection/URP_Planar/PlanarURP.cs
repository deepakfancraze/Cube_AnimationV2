namespace UnityEngine.Rendering.Universal
{
    public class PlanarURP : MonoBehaviour
    {
        public bool VR = false;
        public int ReflectionTexResolution = 512;
        public float Offset = 0.0f;
        [Range(0, 1)]
        public float ReflectionAlpha = 0.5f;
        public bool BlurredReflection;
        public LayerMask LayersToReflect = -1;

        private Camera reflectionCamera;
        private RenderTexture reflectionTexture = null, reflectionTextureRight = null;
        private static bool isRendering = false;
        private Material material;
        private static readonly int reflectionTexString = Shader.PropertyToID("_ReflectionTex");
        private static readonly int reflectionTexRString = Shader.PropertyToID("_ReflectionTexRight");
        private static readonly int reflectionAlphaString = Shader.PropertyToID("_RefAlpha");
        private static readonly string blurString = "BLUR";
        private static readonly string vrString = "VRon";
        private Matrix4x4 reflectionMatrix;
        private Vector4 reflectionPlane;
        private Vector3 posistion;
        private Vector3 normal;
        private Matrix4x4 projection;
        private Vector4 oblique;
        private Matrix4x4 worldToCameraMatrix;
        private Vector3 clipNormal;
        private Vector4 clipPlane;
        private Vector3 oldPosition;
        Vector3 eulerAngles;


        void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += this.RenderObject;
        }


        //private void OnDisable()
        //{
        //    RenderPipelineManager.beginCameraRendering -= this.RenderObject;
        //    if (reflectionTexture)
        //    {
        //        RemoveObject(reflectionTexture);
        //        reflectionTexture = null;
        //    }
        //    if (reflectionTextureRight)
        //    {
        //        RemoveObject(reflectionTextureRight);
        //        reflectionTextureRight = null;
        //    }
        //    if (reflectionCamera)
        //    {
        //        RemoveObject(reflectionCamera.gameObject);
        //        reflectionCamera = null;
        //    }
        //}
        private void OnDisable()
        { 
         RenderPipelineManager.beginCameraRendering -= this.RenderObject;
        }

        public void Start()
        {
            material = GetComponent<Renderer>().sharedMaterials[0];
            QualitySettings.pixelLightCount = 0;

            var go = new GameObject(GetInstanceID().ToString(), typeof(Camera), typeof(Skybox));
            reflectionCamera = go.GetComponent<Camera>();
            var lwrpCamData = go.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
            lwrpCamData.renderShadows = false;
            lwrpCamData.requiresColorOption = CameraOverrideOption.Off;
            lwrpCamData.requiresDepthOption = CameraOverrideOption.Off;
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.cullingMask = ~(1 << 4) & LayersToReflect.value;
            reflectionCamera.cameraType = CameraType.Reflection;

            go.hideFlags = HideFlags.HideAndDontSave;

            if (reflectionTexture)
            {
                RemoveObject(reflectionTexture);
            }

            reflectionTexture = new RenderTexture(ReflectionTexResolution, ReflectionTexResolution, 16)
            {
                isPowerOfTwo = true,
                hideFlags = HideFlags.DontSave
            };

            if (reflectionTextureRight)
            {
                RemoveObject(reflectionTextureRight);
            }

            reflectionTextureRight = new RenderTexture(ReflectionTexResolution, ReflectionTexResolution, 16)
            {
                isPowerOfTwo = true,
                hideFlags = HideFlags.DontSave
            };
        }

        void RenderObject(ScriptableRenderContext context, Camera cam)
        {
            if (transform)
            {
                if (isRendering)
                {
                    return;
                }

                isRendering = true;
                posistion = transform.position;
                normal = transform.up;

                reflectionCamera.clearFlags = cam.clearFlags;
                reflectionCamera.backgroundColor = cam.backgroundColor;
                reflectionCamera.farClipPlane = cam.farClipPlane;
                reflectionCamera.nearClipPlane = cam.nearClipPlane;
                reflectionCamera.orthographic = cam.orthographic;
                reflectionCamera.fieldOfView = cam.fieldOfView;
                reflectionCamera.aspect = cam.aspect;
                reflectionCamera.orthographicSize = cam.orthographicSize;

                if (cam.clearFlags == CameraClearFlags.Skybox)
                {
                    var sky = cam.GetComponent(typeof(Skybox)) as Skybox;
                    var mysky = reflectionCamera.GetComponent(typeof(Skybox)) as Skybox;
                    if (!sky || !sky.material)
                    {
                        mysky.enabled = false;
                    }
                    else
                    {
                        mysky.enabled = true;
                        mysky.material = sky.material;
                    }
                }

                reflectionPlane = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, posistion) - Offset);

                reflectionMatrix.m00 = (1F - 2F * reflectionPlane[0] * reflectionPlane[0]);
                reflectionMatrix.m01 = (-2F * reflectionPlane[0] * reflectionPlane[1]);
                reflectionMatrix.m02 = (-2F * reflectionPlane[0] * reflectionPlane[2]);
                reflectionMatrix.m03 = (-2F * reflectionPlane[3] * reflectionPlane[0]);
                reflectionMatrix.m10 = (-2F * reflectionPlane[1] * reflectionPlane[0]);
                reflectionMatrix.m11 = (1F - 2F * reflectionPlane[1] * reflectionPlane[1]);
                reflectionMatrix.m12 = (-2F * reflectionPlane[1] * reflectionPlane[2]);
                reflectionMatrix.m13 = (-2F * reflectionPlane[3] * reflectionPlane[1]);
                reflectionMatrix.m20 = (-2F * reflectionPlane[2] * reflectionPlane[0]);
                reflectionMatrix.m21 = (-2F * reflectionPlane[2] * reflectionPlane[1]);
                reflectionMatrix.m22 = (1F - 2F * reflectionPlane[2] * reflectionPlane[2]);
                reflectionMatrix.m23 = (-2F * reflectionPlane[3] * reflectionPlane[2]);
                reflectionMatrix.m30 = 0F;
                reflectionMatrix.m31 = 0F;
                reflectionMatrix.m32 = 0F;
                reflectionMatrix.m33 = 1F;

                oldPosition = cam.transform.position;
                reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflectionMatrix;

                worldToCameraMatrix = reflectionCamera.worldToCameraMatrix;
                clipNormal = worldToCameraMatrix.MultiplyVector(normal).normalized;
                clipPlane = new Vector4(clipNormal.x, clipNormal.y, clipNormal.z, -Vector3.Dot(worldToCameraMatrix.MultiplyPoint(posistion + normal * Offset), clipNormal));

                if (!VR)
                {
                    RenderObjectCamera(cam.projectionMatrix, false);
                    material.DisableKeyword(vrString);
                    GL.invertCulling = true;
                    reflectionCamera.transform.position = reflectionMatrix.MultiplyPoint(oldPosition);
                    eulerAngles = cam.transform.eulerAngles;
                    reflectionCamera.transform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
                    UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
                    reflectionCamera.transform.position = oldPosition;
                    GL.invertCulling = false;
                    material.SetTexture(reflectionTexString, reflectionTexture);
                }
                else
                {
                    RenderObjectCamera(cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left), false);
                    material.EnableKeyword(vrString);
                    GL.invertCulling = true;
                    reflectionCamera.transform.position = reflectionMatrix.MultiplyPoint(oldPosition);
                    eulerAngles = cam.transform.eulerAngles;
                    reflectionCamera.transform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
                    UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
                    reflectionCamera.transform.position = oldPosition;
                    GL.invertCulling = false;
                    material.SetTexture(reflectionTexString, reflectionTexture);
                    RenderObjectCamera(cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right), true);
                    GL.invertCulling = true;
                    reflectionCamera.transform.position = reflectionMatrix.MultiplyPoint(oldPosition);
                    eulerAngles = cam.transform.eulerAngles;
                    reflectionCamera.transform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
                    UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
                    reflectionCamera.transform.position = oldPosition;
                    GL.invertCulling = false;
                    material.SetTexture(reflectionTexRString, reflectionTextureRight);
                }

                material.SetFloat(reflectionAlphaString, ReflectionAlpha);

                if (BlurredReflection)
                {
                    material.EnableKeyword(blurString);
                }
                else
                {
                    material.DisableKeyword(blurString);
                }

                isRendering = false;
            }
        }

        void RemoveObject(Object obj)
        {
            if (Application.isEditor)
            {
               // DestroyImmediate(obj);
            }
            else
            {
             //   Destroy(obj);
            }
        }

        private void RenderObjectCamera(Matrix4x4 projection, bool right)
        {
            oblique = clipPlane * (2.0F / (Vector4.Dot(clipPlane, projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1.0f, 1.0f))));
            projection[2] = oblique.x - projection[3];
            projection[6] = oblique.y - projection[7];
            projection[10] = oblique.z - projection[11];
            projection[14] = oblique.w - projection[15];
            reflectionCamera.projectionMatrix = projection;
            reflectionCamera.targetTexture = right ? reflectionTextureRight : reflectionTexture;
        }

        private static float sgn(float a)
        {
            return a > 0.0f ? 1.0f : a < 0.0f ? -1.0f : 0.0f;
        }
    }
}
