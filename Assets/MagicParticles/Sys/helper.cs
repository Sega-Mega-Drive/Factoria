//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    // rus: вспомогательный враппер
    public static class Magic
    {
        public static string GetVersion() 
        {
            IntPtr s = MP_Core.Magic_GetVersion();            
            return Marshal.PtrToStringAnsi(s);            
        }

        public static string GetParticlesTypeName(int hmEmitter, int index)
        {
            IntPtr s = MP_Core.Magic_GetParticlesTypeName(hmEmitter, index);
            return Marshal.PtrToStringAnsi(s);
        }

        public static string GetEmitterName(int hmEmitter)
        {
            IntPtr s = MP_Core.Magic_GetEmitterName(hmEmitter);
            return Marshal.PtrToStringAnsi(s);
        }

        public static string FindFirst(int hmFile, ref MP_Core.MAGIC_FIND_DATA data, int mode)
        {
            IntPtr s = MP_Core.Magic_FindFirst(hmFile, ref data, mode);
            return Marshal.PtrToStringAnsi(s);
        }

        public static string FindNext(int hmFile, ref MP_Core.MAGIC_FIND_DATA data)
        {
            IntPtr s = MP_Core.Magic_FindNext(hmFile, ref data);
            return Marshal.PtrToStringAnsi(s);
        }

        public static string GetFileName(int hmFile)
        {
            IntPtr s = MP_Core.Magic_GetFileName(hmFile);
            return Marshal.PtrToStringAnsi(s);
        }

        public static MP_Core.MAGIC_ACTION GetActionIdentity ()
        {
            MP_Core.MAGIC_ACTION action = new MP_Core.MAGIC_ACTION();
            MP_Core.MAGIC_ACTION_Identity(ref action);
            return action;
        }

        public static void SetCamera(Transform transform, MP_Core.MAGIC_CAMERA_ENUM mode = MP_Core.MAGIC_CAMERA_ENUM.MAGIC_CAMERA_PERSPECTIVE)
        {
            MP_Core.MAGIC_CAMERA camera = new MP_Core.MAGIC_CAMERA();            
            camera.mode = mode;

            Vector3 v = transform.position;
            camera.pos.x = v.x;
            camera.pos.y = v.y;
            camera.pos.z = v.z;            

            v = transform.forward;
            camera.dir.x = v.x;
            camera.dir.y = v.y;
            camera.dir.z = v.z;
                        
            MP_Core.Magic_SetCamera(ref camera);
        }

        public static MP_Core.MAGIC_CAMERA GetCamera ()
        {
            MP_Core.MAGIC_CAMERA camera = new MP_Core.MAGIC_CAMERA();
            MP_Core.Magic_GetCamera(ref camera);
            return camera;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct ColorWrap
        {
            [FieldOffset(0)]
            public Color32 color32;

            [FieldOffset(0)]
            public int colorInt;
        };

        public static unsafe Color32 GetTint (int hmEmitter)
        {
            ColorWrap col = new ColorWrap();
            col.colorInt = MP_Core.Magic_GetTint(hmEmitter);

            return col.color32;            
        }

        public static unsafe int SetTint(int hmEmitter, Color32 color)
        {
            ColorWrap col = new ColorWrap();
            col.color32 = color;

            return MP_Core.Magic_SetTint(hmEmitter, col.colorInt);
        }

        public static Quaternion SetDirection(this Quaternion q, MP_Core.MAGIC_DIRECTION pos)
        {
            q.x = pos.x;
            q.y = pos.y;
            q.z = pos.z;
            q.w = pos.w;
            return q;
        }

        public static Quaternion QuaternionPlus(Quaternion q1, Quaternion q2)
        {
            Quaternion q = new Quaternion();
            q.x = q1.x + q2.x;
            q.y = q1.y + q2.y;
            q.z = q1.z + q2.z;
            q.w = q1.w + q2.w;
            return q;
        }

        public static Quaternion QuaternionMinus (Quaternion q1, Quaternion q2)
        {
            Quaternion q = new Quaternion();
            q.x = q1.x - q2.x;
            q.y = q1.y - q2.y;
            q.z = q1.z - q2.z;
            q.w = q1.w - q2.w;
            return q;
        }

        public static MP_Core.MAGIC_DIRECTION ToDirection(this Quaternion q)
        {
            MP_Core.MAGIC_DIRECTION pos = new MP_Core.MAGIC_DIRECTION();
            pos.x = q.x;
            pos.y = q.y;
            pos.z = q.z;
            pos.w = q.w;
            return pos;
        }

        public static unsafe Hashtable GetVariables (int emitter)
        {
            Hashtable table = new Hashtable();

            // Получаем список переменных заданных в редакторе MagicParticles
            MP_Core.MAGIC_VARIABLE var = new MP_Core.MAGIC_VARIABLE();
            int count = MP_Core.Magic_GetEmitterVariableCount(emitter);
            for (int i = 0; i < count; i++)
            {
                MP_Core.Magic_GetEmitterVariable(emitter, i, ref var);

                string key = var.Name();
                object val = null;

                switch (var.type)
                {
                    case MP_Core.MAGIC_VARIABLE_ENUM.MAGIC_VARIABLE_BOOL:
                        val = *(bool*)var.value;
                        break;

                    case MP_Core.MAGIC_VARIABLE_ENUM.MAGIC_VARIABLE_INT:
                        val = *(Int32*)var.value;
                        break;

                    case MP_Core.MAGIC_VARIABLE_ENUM.MAGIC_VARIABLE_FLOAT:
                    case MP_Core.MAGIC_VARIABLE_ENUM.MAGIC_VARIABLE_DIAGRAM:
                        val = *(float*)var.value;
                        break;

                    case MP_Core.MAGIC_VARIABLE_ENUM.MAGIC_VARIABLE_STRING:
                        val = new string(*(sbyte**)var.value);
                        break;

                    default:
                        Debug.LogError("[MagicParticles] unknown variable type width index: " + i + " " + key);
                        continue;
                }

                table.Add(key, val);
            }

            return table;
        }

        public static bool CreateTriangles(Collider collider, ref MP_Core.MAGIC_OBSTACLE obstacle, ref MP_Core.MAGIC_TRIANGLE[] triangles)
        {
            if (collider is BoxCollider)
            {
                obstacle.type = MP_Core.MAGIC_OBSTACLE_ENUM.MAGIC_OBSTACLE_TRIANGLE;
                obstacle.count = 12;

                Matrix4x4 mat = collider.transform.localToWorldMatrix;
                BoxCollider box = collider as BoxCollider;
                Vector3 half = box.size / 2.0f;
                                
                // top
                triangles[0].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, -half.z));
                triangles[0].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, half.z));
                triangles[0].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[1].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[1].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, -half.z));
                triangles[1].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, -half.z));

                // bottom
                triangles[2].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));
                triangles[2].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, half.z));
                triangles[2].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, half.z));
                triangles[3].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, half.z));
                triangles[3].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, -half.z));
                triangles[3].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));

                // left
                triangles[4].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));
                triangles[4].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, half.z));
                triangles[4].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, half.z));
                triangles[5].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, half.z));
                triangles[5].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, -half.z));
                triangles[5].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));

                // right
                triangles[6].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, -half.z));
                triangles[6].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, half.z));
                triangles[6].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[7].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[7].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, -half.z));
                triangles[7].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, -half.z));

                // near
                triangles[8].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));
                triangles[8].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, -half.z));
                triangles[8].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, -half.z));
                triangles[9].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, -half.z));
                triangles[9].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, -half.z));
                triangles[9].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, -half.z));

                // far
                triangles[10].vertex1 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, half.z));
                triangles[10].vertex2 = mat.MultiplyVector(box.center + new Vector3(-half.x, half.y, half.z));
                triangles[10].vertex3 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[11].vertex1 = mat.MultiplyVector(box.center + new Vector3(half.x, half.y, half.z));
                triangles[11].vertex2 = mat.MultiplyVector(box.center + new Vector3(half.x, -half.y, half.z));
                triangles[11].vertex3 = mat.MultiplyVector(box.center + new Vector3(-half.x, -half.y, half.z));
            }
            else if (collider is SphereCollider)
            {                
                obstacle.count = 0;
                obstacle.type = MP_Core.MAGIC_OBSTACLE_ENUM.MAGIC_OBSTACLE_SPHERE;
                obstacle.radius = (collider as SphereCollider).radius * collider.transform.lossyScale.x;
            }
            else
            {
                Debug.LogWarning("[MagicParticles] Unsupport collider type: " + collider.GetType());
                return false;
            }

            return true;
        }
    }

    public class MP_MATERIAL
    {
        public MP_Core.MAGIC_MATERIAL mat_info;
        public Material material;

        public static unsafe string GenerateName(ref MP_Core.MAGIC_MATERIAL info)
        {
            // rus: формирование кодового обозначения материала

            // flags = MAGIC_MATERIAL_ALPHATEST | MAGIC_MATERIAL_ZWRITE
            string s = "MP" + (int)info.textures + (int) (info.flags & 0x05);

            // rus: параметры формирования шейдеров
            for (int i = 0; i < info.textures; i++)
            {
                MP_Core.MAGIC_TEXTURE_STATES st = info.states[i];

                //s += (int)st.address_u;
                //s += (int)st.address_v;

                s += (int)st.operation_rgb;
                s += (int)st.argument_rgb1;
                s += (int)st.argument_rgb2;

                s += (int)st.operation_alpha;
                s += (int)st.argument_alpha1;
                s += (int)st.argument_alpha2;                
            }

            return s;
        }

        public MP_MATERIAL(ref MP_Core.MAGIC_MATERIAL m)
        {
            mat_info = m;

            string shaderName = GenerateName(ref mat_info);

            Shader shader = Shader.Find("Hidden/" + shaderName);
            if (shader == null)
            {
#if UNITY_EDITOR
                CreateShader(mat_info, shaderName);
                shader = Shader.Find("Hidden/" + shaderName);
                if (shader == null)
#endif
                    Debug.LogError("[MP_MATERIAL] Can't find shader: " + shaderName);
            }

            material = new Material(shader);
            material.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
            material.name = shaderName;
        }

#if UNITY_EDITOR
        // rus: генерация шейдера
        public static unsafe void CreateShader(MP_Core.MAGIC_MATERIAL m, string name)
        {
            // Заголовок
            string s = "Shader \"Hidden/" + name + "\" {" + @"
	SubShader {		

		Tags { 
			""Queue""=""Transparent+1"" 
			""IgnoreProjector""=""True"" 
			""RenderType""=""Transparent"" 
		}				

		Pass {
            Blend [_srcBlend] [_dstBlend]
            ZTest [_ZTest] // LEqual default
"; 

            int f = m.flags;
            if ((f & MP_Core.MAGIC_MATERIAL_ALPHATEST) > 0)
            {
                s += "\t\t\tAlphaTest Greater .01\n";
            }                        
            if ((f & MP_Core.MAGIC_MATERIAL_ZWRITE) == 0)
            {
                s += "\t\t\tZWrite Off\n";
            }

            s += @"
            ColorMask ARGB						 			
			Cull Off 
			Lighting Off 		
			Fog {Mode Off}

			CGPROGRAM								

			#pragma vertex vert
			#pragma fragment frag			

			sampler2D _Tex0;
";
            for (int i=1; i<m.textures; i++)            
                s += "\t\t\tsampler2D _Tex" + i + ";\n";

            // Входная 
            s += @"
            struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord0 : TEXCOORD0;
";

            for (int i = 1; i < m.textures; i++)
                s += "\t\t\t\tfloat2 texcoord" + i + " : TEXCOORD" + i + ";\n";

            // Внутренняя
            s += @"
            };

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR0;
				float2 uv0 : TEXCOORD0;
";

            for (int i = 1; i < m.textures; i++)
                s += "\t\t\t\tfloat2 uv" + i + " : TEXCOORD" + i + ";\n";

            // Вершинный шейдер
            s += @"
            };

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_VP, v.vertex);
				o.color = v.color;
				o.uv0 = v.texcoord0; o.uv0.y = 1.0 - o.uv0.y;
";

            for (int i = 1; i < m.textures; i++)
                s += String.Format("\t\t\t\to.uv{0:d} = v.texcoord{0:d}; o.uv{0:d}.y = 1.0 - o.uv{0:d}.y;\n", i);

            // Пиксельный шейдер
            s += @"
                return o;
            }            

			fixed4 frag (v2f i) : SV_Target
			{
                //_Tex0 = _MainTex;

                fixed4 color;
                fixed4 arg1;
                fixed4 arg2;
                fixed4 colorTex;

                ";

            for (int i = 0; i < m.textures; i++)
            {
                MP_Core.MAGIC_TEXTURE_STATES st = m.states[i];                

                if (st.argument_rgb1 == MP_Core.MAGIC_TEXARG_TEXTURE
                    || st.argument_alpha1 == MP_Core.MAGIC_TEXARG_TEXTURE
                    || (st.operation_rgb != MP_Core.MAGIC_TEXOP_ARGUMENT1 && st.argument_rgb2 == MP_Core.MAGIC_TEXARG_TEXTURE)
                    || (st.operation_alpha != MP_Core.MAGIC_TEXOP_ARGUMENT1 && st.argument_alpha2 == MP_Core.MAGIC_TEXARG_TEXTURE)
                    )
                {
                    s += String.Format("colorTex = tex2D(_Tex{0:d}, i.uv{0:d});\n\t\t\t\t", i);                    
                }

                // Первый аргумент
                if (st.argument_rgb1==st.argument_alpha1)
			    {
				    switch (st.argument_rgb1)
				    {
				    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg1 = color;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg1 = i.color;\n\t\t\t\t"; break;
                    default: s += "arg1 = colorTex;\n\t\t\t\t"; break;
                    }				
			    }
			    else
			    {
				    switch (st.argument_rgb1)
				    {
				    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg1.xyz = color.xyz;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg1.xyz = i.color.xyz;\n\t\t\t\t"; break;
                    default: s += "arg1.xyz = colorTex.xyz;\n\t\t\t\t"; break;
				    }

				    switch (st.argument_alpha1)
				    {
				    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg1.w = color.w;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg1.w = i.color.w;\n\t\t\t\t"; break;
                    default: s += "arg1.w = colorTex.w;\n\t\t\t\t"; break;
				    }
			    }


			    if (st.argument_rgb2==st.argument_alpha2 && st.operation_rgb!=MP_Core.MAGIC_TEXOP_ARGUMENT1 && st.operation_alpha!=MP_Core.MAGIC_TEXOP_ARGUMENT1)
			    {
				    switch (st.argument_rgb2)
				    {
				    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg2 = color;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg2 = i.color;\n\t\t\t\t"; break;
                    default: s += "arg2 = colorTex;\n\t\t\t\t"; break;					
				    }
			    }
			    else
			    {
				    if (st.operation_rgb!=MP_Core.MAGIC_TEXOP_ARGUMENT1)
				    {
					    switch (st.argument_rgb2)
					    {
					    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg2.xyz = color.xyz;\n\t\t\t\t"; break;
                        case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg2.xyz = i.color.xyz;\n\t\t\t\t"; break;
                        default: s += "arg2.xyz = colorTex.xyz;\n\t\t\t\t"; break;
					    }
				    }

				    if (st.operation_alpha!=MP_Core.MAGIC_TEXOP_ARGUMENT1)
				    {
					    switch (st.argument_alpha2)
					    {
					    case MP_Core.MAGIC_TEXARG_CURRENT: s += "arg2.w = color.w;\n\t\t\t\t"; break;
                        case MP_Core.MAGIC_TEXARG_DIFFUSE: s += "arg2.w = i.color.w;\n\t\t\t\t"; break;
                        default: s += "arg2.w = colorTex.w;\n\t\t\t\t"; break;
					    }
				    }
			    }

			    if (st.operation_rgb==st.operation_alpha)
			    {   
				    switch (st.operation_rgb)
				    {
				    case MP_Core.MAGIC_TEXOP_ARGUMENT1: s +="color = arg1;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_ADD: s += "color = arg1 + arg2;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_SUBTRACT: s += "color = arg1 - arg2;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_MODULATE: s += "color = arg1 * arg2;\n\t\t\t\t"; break;

				    case MP_Core.MAGIC_TEXOP_MODULATE2X:
                        s += "color = arg1 * arg2;\n\t\t\t\t";
                        s += "color = color + color;\n\t\t\t\t";
					    break;

				    default:
                        s += "color = arg1 * arg2;\n\t\t\t\t";
                        s += "color = color * 4.0;\n\t\t\t\t";
					    break;
				    }
			    }
			    else
			    {
				    switch (st.operation_rgb)
				    {
                    case MP_Core.MAGIC_TEXOP_ARGUMENT1: s += "color.xyz = arg1.xyz;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_ADD: s += "color.xyz = arg1.xyz + arg2.xyz;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_SUBTRACT: s += "color.xyz = arg1.xyz - arg2.xyz;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_MODULATE: s += "color.xyz = arg1.xyz * arg2.xyz;\n\t\t\t\t"; break;

				    case MP_Core.MAGIC_TEXOP_MODULATE2X:
                        s += "color.xyz = arg1.xyz * arg2.xyz;\n\t\t\t\t";
                        s += "color.xyz = color.xyz + color.xyz;\n\t\t\t\t";
					    break;

				    default:
                        s += "color.xyz = arg1.xyz * arg2.xyz;\n\t\t\t\t";
                        s += "color.xyz = color.xyz * 4.0;\n\t\t\t\t";
					    break;
				    }

				    switch (st.operation_alpha)
				    {
                    case MP_Core.MAGIC_TEXOP_ARGUMENT1: s += "color.w = arg1.w;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_ADD: s += "color.w = arg1.w + arg2.w;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_SUBTRACT: s += "color.w = arg1.w - arg2.w;\n\t\t\t\t"; break;
                    case MP_Core.MAGIC_TEXOP_MODULATE: s += "color.w = arg1.w * arg2.w;\n\t\t\t\t"; break;

				    case MP_Core.MAGIC_TEXOP_MODULATE2X:
                        s += "color.w = arg1.w * arg2.w;\n\t\t\t\t";
                        s += "color.w = color.w + color.w;\n\t\t\t\t";
					    break;

				    default:
                        s += "color.w = arg1.w * arg2.w;\n\t\t\t\t";
					    s +="color.w = color.w * 4.0;\n\t\t\t\t";
					    break;
				    }
			    }

                s += "\n\t\t\t\t";
            }

            if (m.textures > 0)
                s += "return color;\n";
            else
                s += "return i.color;\n";

            s += @"
            }
			ENDCG
		}
	}
	FallBack ""Particles/Additive""
}
";
            string mpPath = "";

            if (AssetDatabase.IsValidFolder("Assets/MagicParticles"))
                mpPath = "/MagicParticles";

            if (!AssetDatabase.IsValidFolder("Assets" + mpPath + "/Resources"))
                AssetDatabase.CreateFolder("Assets" + mpPath, "Resources");

            if (!AssetDatabase.IsValidFolder("Assets" + mpPath + "/Resources/mp_shaders"))
                AssetDatabase.CreateFolder("Assets" + mpPath + "/Resources", "mp_shaders");

            string file = Application.dataPath + mpPath + "/Resources/mp_shaders/" + name + ".shader";
            StreamWriter sw = new StreamWriter(file);
            sw.Write(s);
            sw.Flush();
            sw.Dispose();

            AssetDatabase.ImportAsset("Assets" + mpPath + "/Resources/mp_shaders/" + name + ".shader");

            //Debug.LogWarning("[MagicHost] New material was created: " + name);
        }
#endif
    };

    // rus: класс для вычисления хэш кода материала с заданными текстурами
    [StructLayout(LayoutKind.Sequential)]
    struct MaterialHash
    {
        public short material;
        public short tex1;
        public short tex2;
        public short tex3;
        public short tex4;

        public void Clear()
        {
            material = tex1 = tex2 = tex3 = tex4 = 0;
        }

        // rus: вычисление хэш функции для текущей комбинации материала и текстур
        public unsafe int GetHash()
        {
            fixed (short* m = &material)
            {
                return HashFAQ6((byte*)m, sizeof(MaterialHash));
            }
        }

        // rus: сохранение idx текстуры (атласа) для материала
        public unsafe void SetTexture(int unit, int idx)
        {
            fixed (short* t = &tex1)
            {
                *(t + unit) = (short)idx;
            }
        }

        // https://habrahabr.ru/post/219139/
        private unsafe int HashFAQ6(byte* data, int len)
        {
            uint hash = 0;

            for (; len > 0; len--, data++)
            {
                hash += *data;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return (int)hash;
        }
    }

}