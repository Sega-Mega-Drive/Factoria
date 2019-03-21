//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;

using System.IO;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    public class MP_Atlas
    {        
        protected string file_name;
        public string FileName { get { return file_name; } }

        protected Texture2D texture;
        public Texture2D Texture { get { return texture; } }

        public MP_Atlas(int width, int height, string file)
        {
            // Для загрузки текстуры, имя ресурса должно быть без расширения
            file_name = System.IO.Path.GetFileNameWithoutExtension(file);

            if (!string.IsNullOrEmpty(file_name))
            {
                texture = Resources.Load<Texture2D>(Platform.texture_path + file_name);
                if (texture == null)
                {
                    Debug.LogError("[MP_Atlas] Texture not found: " + file_name);
                    return;
                }
            }
            else
            {
                texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;                
                file_name = "Procedural" + Manager.Instance.GetAtlasCount();

                Color32[] cols = new Color32[width * height];                
                texture.SetPixels32(cols);
                texture.Apply();
            }

            texture.wrapMode = TextureWrapMode.Repeat;
        }

        // eng: Destroy atlas texture
        // rus: Уничтожить текстуру атласа
        public void Destroy()
        {
#if !UNITY_EDITOR
            Object.Destroy(texture);
#endif
            texture = null;
        }

        // eng: Loading of frame texture
        // rus: Загрузка текстурного кадра
        public void LoadTexture(MP_Core.MAGIC_CHANGE_ATLAS c)
        {
            if (c.data == System.IntPtr.Zero)
                return;

            { // Проверка формата данных
                int a = Marshal.ReadInt32(c.data);
                int b = Marshal.ReadInt32(c.data, 4);

                // Если это не png и не jpg
                if ((a != 0x474E5089 || b != 0x0A1A0A0D) && (a & 0xFFFFFF)!=0xFFD8FF)
                {
                    Debug.LogError("[MP_Atlas] Error loading texture '" + c.File() + "' failure (not a png/jpg)");
                    return;
                }
            }
                        
            // Prepare image data
            byte[] data = new byte[c.length];
            Marshal.Copy(c.data, data, 0, (int) c.length);

            // Load frame 
            Texture2D sub = new Texture2D(2, 2);
            sub.LoadImage(data);            

            if (sub.width == 0 || sub.height == 0)
                return;

            // Copy pixels of image
            Color[] pixels = sub.GetPixels(0);
                        
            bool is_scaled=(sub.width!=c.width || sub.height!=c.height);
	        if (is_scaled)
	        {
		        // масштабируем текстуру		        
		        Color[] to= new Color[c.width*c.height];

		        float scale_x=sub.width / (float)c.width;
		        float scale_y=sub.height / (float)c.height;

		        int pitch_to=c.width;
		        int pitch_from=sub.width;
		
		        for (int i=0; i<c.width; i++)
		        {
			        for (int j=0; j<c.height; j++)
			        {
				        int i2=(int) (i * scale_x);
				        int j2=(int) (j * scale_y);

				        to[j*pitch_to+i]=pixels[j2*pitch_from+i2];
			        }
		        }

		        pixels=to;
	        }

            // Paste image
            texture.SetPixels(c.x, texture.height - c.height - c.y, c.width, c.height, pixels);
            texture.Apply();
        }

        // eng: Cleaning up of rectangle
        // rus: Очистка прямоугольника
        public void CleanRectangle(MP_Core.MAGIC_CHANGE_ATLAS a)
        {
            Debug.Assert(false, "[MP_Atlas] CleanRectangle - not implemented");
        }
	}
}