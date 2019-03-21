//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    // eng: Class controlling drawing. 
    // rus: �����, ������� ��������� ����������. 
    public class MP_Device
    {
        private int k_material;
        // rus: ������ ������� ���������� MP (��������� ��� �������� ptc)
        private MP_MATERIAL[] m_material;

        private MaterialHash matCode = new MaterialHash();

        // rus: ��������� �������� ���� ��� �������
        private MP_Core.MAGIC_RENDERING_START ctx_start;
        private MP_Core.MAGIC_RENDER_VERTICES ctx_vrts;
        private MP_Core.MAGIC_RENDER_STATE ctx_state;

        private MP_Core.MAGIC_ARRAY_INFO vertex_info;              
        private MP_Core.MAGIC_ARRAY_INFO index_info;

        // rus: ��������������� ������� ��� ���������� ����� � ��������� ������������
        private List<Material> mats = new List<Material>();
        private Texture[] texs = new Texture[4];


        protected static MP_Device instance;
        public static MP_Device Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MP_Device();
                    instance.Reset();

                    instance.mats.Capacity = 1000;
                }

                return instance;
            }
        }
    	    
	    // rus: ����� �������
        private void Reset() {
            texs[0] = null;
            texs[1] = null;
            texs[2] = null;
            texs[3] = null;

            mats.Clear();            
        }

        public void Clear()
        {
            Reset();
            
            k_material = 0;
            m_material = null;
        }

        public override string ToString ()
        {
            return k_material.ToString();
        }

        // eng: Refreshing materials
        // rus: ���������� ����������
        public void RefreshMaterials()
        {
            Manager.Instance.ClearAllMesh();

	        int k_material_api=MP_Core.Magic_GetMaterialCount();
	        if (k_material<k_material_api)
	        {
                // rus: ���������� ��������� ������ ����������
		        int i;
		        if (m_material != null)
		        {
			        // eng: broadening of material array
			        // rus: ���������� ������� ����������
			        MP_MATERIAL[] vm_material=new MP_MATERIAL[k_material_api];
			        for (i=0;i<k_material;i++)
				        vm_material[i] = m_material[i];
			        
			        m_material=vm_material;
		        }
		        else
		        {
			        m_material=new MP_MATERIAL[k_material_api];
		        }

		        MP_Core.MAGIC_MATERIAL mat = new MP_Core.MAGIC_MATERIAL();

                // rus: ��������� ����� ���������� � �������� ��� ���
		        for (i=k_material;i<k_material_api;i++)
		        {
			        MP_Core.Magic_GetMaterial(i, ref mat);
                    m_material[i] = new MP_MATERIAL(ref mat);
		        }

		        k_material=k_material_api;
	        }
        }

        // eng: Emitter visualization
        // rus: ��������� ��������. ������������ ���������� ������������ ������
        public void UpdateMesh(MP_Emitter emitter, MeshRenderer render, Mesh mesh, EmitterBuffers buffers)
        {
            // rus: ������ ��������� ��� ��������� ������
            IntPtr context = MP_Core.Magic_PrepareRenderArrays(emitter.ID, ref ctx_start, 10, MP_Core.MAGIC_ARGB_ENUM.MAGIC_ARGB, true);
            if (ctx_start.arrays == 0 || context == IntPtr.Zero)
                return;

            // rus: ������ ���������� � �������� ������� � �� ���.
            MP_Core.Magic_GetRenderArrayData(context, 0, ref vertex_info);
            if (vertex_info.length > EmitterBuffers.MAX_BUFFER_SIZE || vertex_info.length==0)
                return;

            // rus: ���������� ������� � ������ �������������
            if (buffers.vrtLength < vertex_info.length || buffers.arrays != ctx_start.arrays)
                buffers.ResizeArrays(vertex_info.length, ctx_start.arrays);

            // rus: ������ � ���������� ���������� ������
            MP_Core.Magic_GetRenderArrayData(context, ctx_start.arrays-1, ref index_info);
            if (buffers.idxLength < index_info.length)
                buffers.ResizeIndexArray(index_info.length);

            unsafe
            {
                fixed (void *v = buffers.vrts)
                    MP_Core.Magic_SetRenderArrayData(context, 0, v, 0, 12);

                fixed (void* c = buffers.cols)
                    MP_Core.Magic_SetRenderArrayData(context, 1, c, 0, 4);

                fixed (void* uv = buffers.uv1)
                    MP_Core.Magic_SetRenderArrayData(context, 2, uv, 0, 8);
                if (ctx_start.arrays > 4) fixed (void* uv = buffers.uv2)
                    MP_Core.Magic_SetRenderArrayData(context, 3, uv, 0, 8);
                if (ctx_start.arrays > 5) fixed (void* uv = buffers.uv3)
                    MP_Core.Magic_SetRenderArrayData(context, 4, uv, 0, 8);
                if (ctx_start.arrays > 6) fixed (void* uv = buffers.uv4)
                    MP_Core.Magic_SetRenderArrayData(context, 5, uv, 0, 8);

                fixed (void* idx = buffers.idx)
                    MP_Core.Magic_SetRenderArrayData(context, ctx_start.arrays - 1, idx, 0, index_info.bytes_per_one);
            }
            /*
            // rus: ������ �������� ��� ���������� ����� api            
            MP_Core.Magic_SetRenderArrayDataV3(context, 0, buffers.vrts, 0, 12);
            MP_Core.Magic_SetRenderArrayDataCol(context, 1, buffers.cols, 0, 4);
            MP_Core.Magic_SetRenderArrayDataV2(context, 2, buffers.uv1, 0, 8);
            if (ctx_start.arrays > 4) MP_Core.Magic_SetRenderArrayDataV2(context, 3, buffers.uv2, 0, 8);
            if (ctx_start.arrays > 5) MP_Core.Magic_SetRenderArrayDataV2(context, 4, buffers.uv3, 0, 8);
            if (ctx_start.arrays > 6) MP_Core.Magic_SetRenderArrayDataV2(context, 5, buffers.uv4, 0, 8);
            MP_Core.Magic_SetRenderArrayDataInt(context, ctx_start.arrays - 1, buffers.idx, 0, index_info.bytes_per_one);
             */

            // rus: ���������� �������� ����������� ������� ��� ������������ �������
            MP_Core.Magic_FillRenderArrays(context);

            // rus: ���������� �������� � �����
            mesh.triangles = null;
            mesh.vertices = buffers.vrts;
            mesh.colors32 = buffers.cols;            

            if (ctx_start.arrays > 3) mesh.uv = buffers.uv1; else mesh.uv = null;
            if (ctx_start.arrays > 4) mesh.uv2 = buffers.uv2; else mesh.uv2 = null;
            if (ctx_start.arrays > 5) mesh.uv3 = buffers.uv3; else mesh.uv3 = null;
            if (ctx_start.arrays > 6) mesh.uv4 = buffers.uv4; else mesh.uv4 = null;

            // rus: ���������� ��� ������������ ��������            
            buffers.ClearSubmesh();
            mats.Clear();
            
            // rus: ��� ��������� ��� ������� �� ����
            matCode.Clear();

            // rus: ������� ����� ��� ������� �� api (������ ������, ���������� ������)
            while (MP_Core.Magic_GetVertices(context, ref ctx_vrts) == MP_Core.MAGIC_SUCCESS)
            {
                matCode.material = (short) ctx_vrts.material;                                

                // rus: �������� �������� ��� ������� ������ ������
                while (MP_Core.Magic_GetNextRenderState(context, ref ctx_state) == MP_Core.MAGIC_SUCCESS)
                {
                    if (ctx_state.state == MP_Core.MAGIC_RENDER_STATE_ENUM.MAGIC_RENDER_STATE_TEXTURE)
                    {
                        matCode.SetTexture(ctx_state.index, ctx_state.value);                        
                        texs[ctx_state.index] = Manager.Instance.GetAtlas(ctx_state.value).Texture;
                    }
                }
                

                // rus: ������� �������� �� ��� ���� ��� ������� ������
                int hash = matCode.GetHash();                
                Material mat = (Material)buffers.cacheMaterial[hash];

                // rus: ��� ����� ��������
                if (mat == null)
                {
                    // rus: ������� ��������
                    MP_MATERIAL def_mat = m_material[ctx_vrts.material];

                    // rus: ����������� ��������
                    mat = new Material(def_mat.material);
                    mat.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;

                    // rus: ��������� ��������� ��� submesh
                    switch (m_material[ctx_vrts.material].mat_info.blending)
                    {
                        case MP_Core.MAGIC_BLENDING_NORMAL:
                            mat.SetInt("_srcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            mat.SetInt("_dstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            break;

                        case MP_Core.MAGIC_BLENDING_ADD:
                            mat.SetInt("_srcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            mat.SetInt("_dstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            break;

                        case MP_Core.MAGIC_BLENDING_OPACITY:
                            mat.SetInt("_srcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            mat.SetInt("_dstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            break;
                    }

                    mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);

                    int texCount = def_mat.mat_info.textures;

                    if (texCount > 0) mat.SetTexture("_Tex0", texs[0]);
                    if (texCount > 1) mat.SetTexture("_Tex1", texs[1]);
                    if (texCount > 2) mat.SetTexture("_Tex2", texs[2]);
                    if (texCount > 3) mat.SetTexture("_Tex3", texs[3]); 

                    // rus: ��������� �������� � ���
                    buffers.cacheMaterial[hash] = mat;
                } // end_if

                mats.Add(mat);

                // rus: ������� �������� ��� ����������� submesh �� ������ ������ ��������
                buffers.UpdateSubmesh(buffers.passCount, ref ctx_vrts);
                buffers.passCount++;
            } // end_while

            // rus: ������������� ������ ���������� ��� ������� �������
            render.materials = mats.ToArray();

            // rus: ������ ������� (������ ������ � ������� �����������)            
            mesh.subMeshCount = mats.Count;
            for (int i = 0; i < buffers.passCount; i++)
                mesh.SetTriangles(buffers.submeshIndices[i], i);

            // rus: �����, ����� AABB ������������ ������������ ������ ���������,
            // �.�. ������ �� ��������� MODEL_MATRIX � ������ ����� � ������� ���������
            Bounds b = mesh.bounds;
            b.center = Vector3.zero;
            mesh.bounds = b;

            buffers.particles = ctx_start.particles;            
        }
    };

    // rus: ������ ��� ������� ������
    public class EmitterBuffers
    {
        const int RESERVE_SIZE = 1023;
        const int BUFFER_SIZE = 1024;
        public const int MAX_BUFFER_SIZE = 65000;

        public int idxLength = BUFFER_SIZE;
        public int vrtLength = BUFFER_SIZE;

        // rus: ���. ��������� ����������
        public int materials = 0;

        // rus: ����� ��������
        public int passCount = 0;

        // rus: ���. ������������ uv ��������  
        public int arrays = 0;

        // rus: ����� ������ � ��������� ����������
        public int particles = 0;

        public Vector3[] vrts = new Vector3[BUFFER_SIZE];
        public Color32[] cols = new Color32[BUFFER_SIZE];
        public Vector2[] uv1 = new Vector2[BUFFER_SIZE];
        public Vector2[] uv2 = new Vector2[1];
        public Vector2[] uv3 = new Vector2[1];
        public Vector2[] uv4 = new Vector2[1];

        // rus: ����� ��������� ������
        public int[] idx = new int[BUFFER_SIZE];

        // rus: ������� �������� ��� ��������
        public List<int[]> submeshIndices = new List<int[]>();

        // rus: ��� ���������� ������������ ��� �������
        public Hashtable cacheMaterial = new Hashtable();

        // rus: ����� ������� �������� � ���������
        public void Reset ()
        {
            arrays = 4;            
            vrtLength = idxLength = BUFFER_SIZE;
            ResizeArrays(vrtLength, arrays);
            ResizeIndexArray(idxLength);
            submeshIndices.Clear();
            cacheMaterial.Clear();
        }

        public void ResizeArrays(int size, int arrays)
        {
            while (vrtLength < size)
                vrtLength <<= 1;

            if (vrtLength > MAX_BUFFER_SIZE)
                vrtLength = MAX_BUFFER_SIZE;

            this.arrays = arrays;
            if (vrts.Length != vrtLength)
            {
                vrts = new Vector3[vrtLength];
                cols = new Color32[vrtLength];
                uv1 = new Vector2[vrtLength];
            }

            if (uv2.Length != vrtLength)
                uv2 = new Vector2[arrays>4?vrtLength:1];

            if (uv3.Length != vrtLength)
                uv3 = new Vector2[arrays>5?vrtLength:1];

            if (uv4.Length != vrtLength)
                uv4 = new Vector2[arrays>6?vrtLength:1];
        }

        public void ResizeIndexArray(int size)
        {
            while (idxLength < size)
                idxLength <<= 1;

            idx = new int[idxLength];
        }

        // rus: ��������� ���� �����
        public void ClearSubmesh ()
        {
            passCount = 0;
            materials = 0;
            particles = 0;
        }

        // rus: ���������� �������� �������
        public void UpdateSubmesh(int index, ref MP_Core.MAGIC_RENDER_VERTICES vrts)
        {
            int[] indices;

            // rus: ���� ������ ��� ������� ����������
            if (index < submeshIndices.Count)
            {                
                indices = submeshIndices[index];

                // rus: ��������� � ������ �������������
                if (vrts.indexes_count > indices.Length)
                {
                    indices = new int[vrts.indexes_count + RESERVE_SIZE];
                    submeshIndices[index] = indices;
                }
            }
            else
            {   // rus: ����� ������ ��� �������
                indices = new int[vrts.indexes_count + RESERVE_SIZE];
                submeshIndices.Add(indices);
            }

            // rus: ����������� �������� �� ������
            Array.Copy(idx, vrts.starting_index, indices, 0, vrts.indexes_count);

            // rus: ��������� ������ ���������� �������
            if (vrts.indexes_count < indices.Length)
                Array.Clear(indices, vrts.indexes_count, indices.Length - vrts.indexes_count);
        }
    }

    
}