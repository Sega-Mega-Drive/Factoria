//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    // eng: Class that is used as storage for Magic Particles emitters
    // rus: Класс, который является хранилищем эмиттеров Magic Particles
    public class Manager
    {
        // eng: number of loaded emitters
        // rus: количество загруженных эмиттеров
        private int k_emitter;
        private int max_emitter;

        // eng: list of loaded emitters
        // rus: список загруженных эмиттеров
        private MP_Emitter[] m_emitter;
        private int[] m_descriptor;

        // rus: список исходных загруженных эмиттеров (без учета дубликатов)
        public Hashtable Emitters;

        private Hashtable m_obstacles;

        // eng: number of loaded atlases
        // rus: количество загруженных атласов
        private int k_atlas;

        // eng: list of loaded atlases
        // rus: список загруженных атласов
        private MP_Atlas[] m_atlas;

        // eng: number of files with particle copies
        // rus: количество файлов с копиями частиц
        private int k_copy;

        // eng: list of files with particle copies
        // rus: список файлов с копиями частиц
        private MP_Copy[] m_copy;

        //---------------------------------------------------------------
        // eng: settings to initialize emitters by default
        // rus: настройки для инициализации эмиттеров по умолчанию
        //---------------------------------------------------------------

        // eng: method of interpolation applied to loaded emitters
        // rus: способ применения интерполяции к загружаемым эмиттерам
        private int interpolation;

        // eng: mode of changing emitter position
        // rus: режим изменения позиции эмиттера
        private int position_mode;

        private int atlas_width, atlas_height;
        private int atlas_frame_step;
        private float atlas_scale_step;

        // eng: mode for working with files containing particle copies. false - file is deleted, when there are no emitters, that use it, true - file is deleted at program termination
        // rus: режим работы файлов с копиями частиц с копиями. false - файл удаляется, когда нет эмиттеров, которые его используют, true - файл удаляется при завершении работы программы
        private bool copy_mode;
        // eng: Getting mode of working with files containing particle copies
        // rus: Получение режима работы файлов копий частиц
        public bool CopyMode { get { return copy_mode; } }

        private int next_descriptor;
        private int next_index;

        private bool is_new_atlas;

        private MP_Core.MAGIC_OBSTACLE obstacle = new MP_Core.MAGIC_OBSTACLE();
        private MP_Core.MAGIC_TRIANGLE[] triangles = new MP_Core.MAGIC_TRIANGLE[12];

        // Текущий открываемый файл
        private string cur_file;

        private static Manager instance;

        private MP_Emitter.EnumNamingMode namingMode = MP_Emitter.EnumNamingMode.Ptc_Name;
        public MP_Emitter.EnumNamingMode NamingMode
        {
            get { return namingMode; }
            set
            {
                if (namingMode==value)
                    return;
                
                // Обновление хеш-таблицы имен эффектов
                Hashtable table = new Hashtable();
                foreach (DictionaryEntry item in Emitters)
                {
                    MP_Emitter emit = (item.Value as MP_Emitter);
                    table[emit.GetFullName(value)] = emit;
                }
                Emitters = table;

                // Обновленние всех связанных эффектов
                if (m_emitter!=null) for (int i = 0; i < m_emitter.Length; i++)
                {
                    MP_Emitter em = m_emitter[i];
                    if (em != null && em.Owner != null)
                        em.Owner.UpdateEmitterName(value);
                }

                namingMode = value;
            }
        }

        public static Manager Instance
        {
            get
            {
                if (instance == null)
                    instance = new Manager();

                return instance;
            }
        }

        private Manager()
        {
            Init();
        }
                
        public void Init()
        {   
            k_emitter = 0;
            max_emitter = 10;

            m_obstacles = new Hashtable();

            Emitters = new Hashtable();
            m_emitter = new MP_Emitter[max_emitter];
            m_descriptor = new int[max_emitter];

            k_atlas = 0;
            m_atlas = null;

            k_copy = 0;
            m_copy = null;

            interpolation = MP_Core.MAGIC_INTERPOLATION_ENABLE;
            position_mode = MP_Core.MAGIC_CHANGE_EMITTER_DEFAULT;

            atlas_width = atlas_height = 1024;
            atlas_frame_step = 1;
            atlas_scale_step = 0.01f;

            next_descriptor = 0;
            next_index = -1;

            is_new_atlas = false;
        }

        // eng: Cleaning up
        // rus: Очистка
        public void Clear()
        {
            MP_Device.Instance.Clear();

            for (int i = 0; i < max_emitter; i++)
            {
                if (m_emitter[i] != null)
                {
                    m_emitter[i].Destroy();
                    m_emitter[i] = null;
                }
            }

            m_obstacles = null;

            m_emitter = null;
            m_descriptor = null;
            max_emitter = 0;

            m_atlas = null;
            k_atlas = 0;

            for (int i = k_copy - 1; i >= 0; i--)
            {
                m_copy[i].Destroy();
            }

            m_copy = null;
            k_copy = 0;

            MP_Core.Magic_UnloadAllEmitters();
            MP_Core.Magic_DestroyAllPhysicObjects(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_WIND);
            MP_Core.Magic_DestroyAllPhysicObjects(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_MAGNET);
            MP_Core.Magic_DestroyAllPhysicObjects(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_OBSTACLE);            
            MP_Core.Magic_CloseAllFiles();            
        }

        // eng: Initialization
        // rus: Инициализация
        public void Initialization(byte[] render_states_filters, bool render_states_optimization, 
            MP_Core.MAGIC_AXIS_ENUM axis, int interpolation, int position_mode, 
            int atlas_width = 1024, int atlas_height = 1024, int atlas_frame_step = 1, 
            float atlas_starting_scale = 1.0f, float atlas_scale_step = 0.1f, bool copy_mode = true)
        {            
            MP_Core.Magic_SetRenderStateFilter(render_states_filters, render_states_optimization);

            MP_Core.Magic_SetAxis(axis);
            MP_Core.Magic_SetStartingScaleForAtlas(atlas_starting_scale);            

            this.interpolation = interpolation;
            this.position_mode = position_mode;

            this.atlas_width = atlas_width;
            this.atlas_height = atlas_height;
            this.atlas_frame_step = atlas_frame_step;
            this.atlas_scale_step = atlas_scale_step;

            this.copy_mode = copy_mode;
        }

        // eng: Returning the number of emitters
        // rus: Возвращение количества эмиттеров
        public int GetEmitterCount() { return k_emitter; }

        // eng: Returning descriptor of first emitter. 
        // rus: Получение дескриптора первого эмиттера
        public int GetFirstEmitter()
        {
            next_descriptor = 0;
            next_index = -1;

            if (k_emitter > 0)
            {
                next_descriptor = m_descriptor[0];
                next_index = 0;
            }

            return next_descriptor;
        }

        // eng: Returning descriptor of next emitter. 
        // rus: Получение дескриптора следующего эмиттера
        public int GetNextEmitter(int hmEmitter)
        {
            if (next_index == -1 || hmEmitter != next_descriptor)
            {
                next_index = -1;
                for (int i = 0; i < k_emitter; i++)
                {
                    if (m_descriptor[i] == hmEmitter)
                    {
                        next_index = i;
                        break;
                    }
                }
            }

            next_descriptor = 0;

            if (next_index != -1)
            {
                next_index++;
                if (next_index < k_emitter)                
                    next_descriptor = m_descriptor[next_index];                
                else
                    next_index = -1;
            }

            return next_descriptor;
        }

        // eng: Returning the emitter by its descriptor
        // rus: Возвращение эмиттера по дескриптору
        public MP_Emitter GetEmitter(int hmEmitter)
        {
            int idx = (int)hmEmitter;
            if (idx >= 0 && idx < max_emitter)
                return m_emitter[idx];

            return null;
        }

        public MP_Emitter Find(string name)
        {
            if (Emitters.Contains(name))
                return (MP_Emitter)Emitters[name];
                        
            foreach (DictionaryEntry p in Emitters)
            {
                MP_Emitter em = (p.Value as MP_Emitter);

                if (NamingMode != MP_Emitter.EnumNamingMode.Ptc_Path_Name && name == em.GetFullName(MP_Emitter.EnumNamingMode.Ptc_Path_Name))
                    return em;

                if (NamingMode != MP_Emitter.EnumNamingMode.Ptc_Name && name == em.GetFullName(MP_Emitter.EnumNamingMode.Ptc_Name))
                    return em;

                if (NamingMode != MP_Emitter.EnumNamingMode.Name && name == em.GetFullName(MP_Emitter.EnumNamingMode.Name))
                    return em;
            }

            return null;
        }

        // eng: Returning the emitter by name
        // rus: Возвращание эмиттера по имени
        public MP_Emitter GetEmitterByName(string name)
        {
            int hmEmitter = GetFirstEmitter();

            while (hmEmitter != 0)
            {
                MP_Emitter emitter = GetEmitter(hmEmitter);

                // eng: name coincides
                // rus: имя совпадает
                if (emitter.Name.CompareTo(name) == 0)
                    return emitter;

                hmEmitter = GetNextEmitter(hmEmitter);
            }

            return null;
        }

        // eng: Loading all the emitters and animated folders from the file specified
        // rus: Загрузка всех эмиттеров из указанного файла. Загружаются эмиттеры и анимированные папки
        public int LoadEmittersFromFile(string file)
        {
            if (!File.Exists(file))
                return 0;

            byte[] data = File.ReadAllBytes(file);
            return LoadEmittersFromFileInMemory(data, file);
        }

        public int LoadEmittersFromFileInMemory(byte[] address, string group = "")
        {
            int hFile = MP_Core.Magic_OpenFileInMemory(address);
            if (hFile == 0)
                return 0;

            // Назначает группу для загружаемых эмиттеров
            cur_file = group;
            LoadFolder(hFile, null);
            CloseFile(hFile);
            cur_file = "";
            return hFile;
        }

        // eng: Deleting specified emitter 
        // rus: Удаление указанного эмиттера
        public int DeleteEmitter(int hmEmitter)
        {
            int result = MP_Core.MAGIC_ERROR;

            next_descriptor = 0;
            next_index = -1;

            for (int j = 0; j < k_emitter; j++)
            {
                int hme = m_descriptor[j];
                if (hme == hmEmitter)
                {
                    // it is necessary to delete this element from index array
                    // нужно удалить данный элемент из индексного массива
                    for (int k = j + 1; k < k_emitter; k++)
                    {
                        m_descriptor[k - 1] = m_descriptor[k];
                    }
                    k_emitter--;

                    m_descriptor[k_emitter] = 0;

                    // rus: удалить из массива эмиттеров
                    MP_Emitter emitter = m_emitter[hmEmitter];
                    m_emitter[hmEmitter] = null;

                    // rus: удалить из списка оригинальных
                    if (Emitters.ContainsValue(emitter))
                        Emitters.Remove(emitter.GetFullName(NamingMode));

                    // rus: уничтожить экземпляр
                    emitter.Destroy();
                    emitter = null;

                    result = MP_Core.MAGIC_SUCCESS;
                    break;
                }
            }

            return result;
        }

        // rus: Удаление эмиттеров из файла
        public void DeleteEmitters(string file)
        {
            next_descriptor = 0;
            next_index = -1;

            for (int j = max_emitter - 1; j >= 0; j--)
            {
                MP_Emitter emit = m_emitter[j];
                if (emit != null && emit.file == file)
                    DeleteEmitter(emit.ID);
            }

            RefreshAtlas();
			//Debug.Log("[MP_Manager] Delete emitters with file: " + file);
        }

        // eng: Closing file
        // rus: Выгрузка одного файла
        public int CloseFile(int hmFile)
        {
            RefreshAtlas();
            return MP_Core.Magic_CloseFile(hmFile);
        }

        // eng: Closing all files
        // rus: Выгрузка всех файлов
        public void CloseFiles()
        {
            RefreshAtlas();
            MP_Core.Magic_CloseAllFiles();
        }

        // eng: Duplicating specified emitter
        // rus: Дублирование указанного эмиттера
        public MP_Emitter DuplicateEmitter(MP_Emitter hmEmitter)
        {
            MP_Emitter from = hmEmitter;
            if (from == null)
                return null;

            if (MP_Core.Magic_IsInterval1(hmEmitter.ID) && from.copy == null)
            {
                // eng: it is necessary firstly to create particles copy
                // rus: необходимо сначала создать копию частиц
                from.Restart();
            }

            MP_Emitter emitter = new MP_Emitter(from);
            AddEmitter(emitter);

            return emitter;
        }

        // rus: обновление зарегистрированных препядствий        
        public void Update()
        {
            if (m_obstacles!=null)
            {
                Collider del = null;
                bool need = false;

                // rus: перебор всех препятствий
                foreach (DictionaryEntry obs in m_obstacles)
                {
                    Collider col = (obs.Key as Collider);
                    if (col != null)
                    {
                        // rus: обновить позицию препятствия
                        MP_Core.MAGIC_POSITION pos = col.transform.position;                        
                        MP_Core.Magic_SetObstaclePosition((int)obs.Value, ref pos);
                        // rus: обновить форму препятствия (повороты, размеры)
                        UpdateObstacle(col, (int)obs.Value);
                    }
                    else
                    {
                        need = true;
                        del = col;
                        MP_Core.Magic_DestroyPhysicObject(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_OBSTACLE, (int)obs.Value);
                    }
                }

                if (need)
                    m_obstacles.Remove(del);
            }
        }

        // rus: обновить все эффекты
        public void UpdateAllMesh()
        {
            if (m_emitter!=null) for (int i = 0; i < m_emitter.Length; i++)
            {
                MP_Emitter em = m_emitter[i];
                if (em != null && em.Owner != null && em.Owner.enabled)
                    em.Owner.Update();
            }
        }

        public void ClearAllMesh()
        {
            if (m_emitter != null) for (int i = 0; i < m_emitter.Length; i++)
            {
                MP_Emitter em = m_emitter[i];
                if (em != null && em.Owner != null)
                    em.Owner.ClearMesh();
            }
        }

        // eng: Refreshing textural atlases
        // rus: Построение текстурного атласа
        public void RefreshAtlas()
        {
            int i;

            if (is_new_atlas)
            {
                // eng: new emitters were added, it is necessary to create new atlases for them
                // rus: были добавлены новые эмиттеры, необходимо создать для них атласы
                is_new_atlas = false;

                int k = GetEmitterCount();
                if (k > 0)
                {
                    int[] hm_emitter = new int[k];

                    k = 0;

                    int hmEmitter = GetFirstEmitter();
                    while (hmEmitter != 0)
                    {
                        MP_Emitter emitter = GetEmitter(hmEmitter);
                        if (!emitter.is_atlas)
                        {
                            emitter.is_atlas = true;
                            hm_emitter[k] = (int) hmEmitter;
                            k++;
                        }

                        hmEmitter = GetNextEmitter(hmEmitter);
                    }

                    if (k != 0)
                        MP_Core.Magic_CreateAtlasesForEmitters(
                            atlas_width,
                            atlas_height,
                            k,
                            hm_emitter,
                            atlas_frame_step,
                            atlas_scale_step
                        );

                    hm_emitter = null;
                }
            }

            MP_Core.MAGIC_CHANGE_ATLAS c = new MP_Core.MAGIC_CHANGE_ATLAS();
            while (MP_Core.Magic_GetNextAtlasChange(ref c) == MP_Core.MAGIC_SUCCESS)
            {
                // Обработка очереди сообщений, при полной очистки менеджера (Unity3d)
                if (m_atlas == null && c.type != MP_Core.MAGIC_CHANGE_ATLAS_ENUM.MAGIC_CHANGE_ATLAS_CREATE)
                    continue;

                switch (c.type)
                {
                    // eng: loading of frame in atlas
                    // rus: загрузка кадра в атлас
                    case MP_Core.MAGIC_CHANGE_ATLAS_ENUM.MAGIC_CHANGE_ATLAS_LOAD:
                        m_atlas[c.index].LoadTexture(c);
                        break;

                    // eng: cleaning up of rectangle in atlas
                    // rus: очистка прямоугольника в атласе
                    case MP_Core.MAGIC_CHANGE_ATLAS_ENUM.MAGIC_CHANGE_ATLAS_CLEAN:
                        m_atlas[c.index].CleanRectangle(c);
                        break;

                    // eng: creating of atlas
                    // rus: создание атласа
                    case MP_Core.MAGIC_CHANGE_ATLAS_ENUM.MAGIC_CHANGE_ATLAS_CREATE:
                        if (m_atlas != null)
                        {
                            // eng: broadening of atlas array
                            // rus: расширение массив атласов
                            MP_Atlas[] vm_atlas = new MP_Atlas[k_atlas + 1];
                            for (i = 0; i < k_atlas; i++)
                                vm_atlas[i] = m_atlas[i];
                            m_atlas = vm_atlas;
                        }
                        else
                        {
                            m_atlas = new MP_Atlas[1];
                        }
                                                
                        m_atlas[k_atlas] = new MP_Atlas(c.width, c.height, c.File());
           
                        k_atlas++;
                        break;

                    // eng: Deleting of atlas
                    // rus: удаление атласа
                    case MP_Core.MAGIC_CHANGE_ATLAS_ENUM.MAGIC_CHANGE_ATLAS_DELETE:
                        m_atlas[c.index].Destroy();

                        if (k_atlas == 1)
                        {
                            m_atlas = null;
                        }
                        else
                        {
                            MP_Atlas[] vm_atlas = new MP_Atlas[k_atlas - 1];
                            for (i = 0; i < c.index; i++)
                                vm_atlas[i] = m_atlas[i];

                            for (i = c.index + 1; i < k_atlas; i++)
                                vm_atlas[i - 1] = m_atlas[i];
                            m_atlas = vm_atlas;
                        }

                        k_atlas--;
                        break;
                }
            }

            MP_Device.Instance.RefreshMaterials();
        }

        // eng: Returns the number of textural atlases
        // rus: Возвращает количество текстурных атласов
        public int GetAtlasCount() { return k_atlas; }

        // eng: Returns textural atlas
        // rus: Возвращает текстурный атлас
        public MP_Atlas GetAtlas(int index) { 
            return m_atlas[index]; 
        }

        public Texture2D[] GetTextures ()
        {
            if (k_atlas == 0)
                return null;

            Texture2D[] texs = new Texture2D[k_atlas];
            for (int i=0; i<k_atlas; i++)
            {
                texs[i] = m_atlas[i].Texture;
            }

            return texs;
        }

        // eng: Adding new emitter into array
        // rus: Добавление нового эмиттера в массив
        public void AddEmitter(MP_Emitter emitter)
        {
            int i;

            next_descriptor = 0;
            next_index = -1;

            int index = (int) emitter.ID;

            while (index >= max_emitter)
            {
                int new_max_emitter = max_emitter + 10;

                MP_Emitter[] vm_emitter = new MP_Emitter[new_max_emitter];
                for (i = 0; i < max_emitter; i++)
                    vm_emitter[i] = m_emitter[i];
                m_emitter = vm_emitter;

                int[] vm_descriptor = new int[new_max_emitter];
                for (i = 0; i < max_emitter; i++)
                    vm_descriptor[i] = m_descriptor[i];
                m_descriptor = vm_descriptor;

                for (i = max_emitter; i < new_max_emitter; i++)
                {
                    m_emitter[i] = null;
                    m_descriptor[i] = 0;
                }

                max_emitter = new_max_emitter;
            }

            m_emitter[index] = emitter;
            m_descriptor[k_emitter] = index;
            k_emitter++;
        }
        
        // eng: Loading emitter
        // rus: Загрузка конкретного эмиттера
        public MP_Emitter LoadEmitter(int hFile, string path)
        {
            // eng: it is necessary to load emitter from file
            // rus: нужно извлечь эмиттер из файла
            MP_Emitter em = null;

            int emitter = MP_Core.Magic_LoadEmitter(hFile, path);
            if (emitter != 0)
            {
                em = new MP_Emitter(emitter);
                em.path = System.IO.Path.GetDirectoryName(path);           
                em.file = cur_file;

                Emitters[em.GetFullName(NamingMode)] = em;
                AddEmitter(em);

                // eng: initialization of emitter by default values
                // rus: инициализация эмиттера значениями по умолчанию
                if (interpolation != MP_Core.MAGIC_INTERPOLATION_DEFAULT)                
                    MP_Core.Magic_SetInterpolationMode(emitter, interpolation == MP_Core.MAGIC_INTERPOLATION_ENABLE);

                switch (position_mode)
                {
                case MP_Core.MAGIC_CHANGE_EMITTER_ONLY:                        
                    MP_Core.Magic_SetEmitterPositionMode(emitter, false);
                    MP_Core.Magic_SetEmitterDirectionMode(emitter, false);
                    break;                        

                case MP_Core.MAGIC_CHANGE_EMITTER_AND_PARTICLES:                        
                    MP_Core.Magic_SetEmitterPositionMode(emitter, true);
                    MP_Core.Magic_SetEmitterDirectionMode(emitter, true);
                    break;                        
                }

                if (MP_Core.Magic_GetStaticAtlasCount(hFile) != 0)
                    em.is_atlas = true;
                else
                    is_new_atlas = true;

#if UNITY_EDITOR
                //Debug.Log("[MP_Manager] Load emitter: " + em.Name);
#endif
            }

            return em;
        }

        // eng: Loading folder
        // rus: Загрузка папки
        public void LoadFolder(int hFile, string path)
        {
            MP_Core.Magic_SetCurrentFolder(hFile, path);
            
            MP_Core.MAGIC_FIND_DATA find = new MP_Core.MAGIC_FIND_DATA();
            
            string pName = Magic.FindFirst(hFile, ref find, MP_Core.MAGIC_FOLDER | MP_Core.MAGIC_EMITTER);            
            while (pName != null)
            {
                if (find.animate != 0)                
                    LoadEmitter(hFile, pName);                
                else                
                    LoadFolder(hFile, pName);                

                pName = Magic.FindNext(hFile, ref find);
            }

            MP_Core.Magic_SetCurrentFolder(hFile, "..");            
        }

        // eng: Adding file with particles copy
        // rus: Добавление файла с копией частиц
        public MP_Copy AddCopy(MP_Emitter emitter)
        {
            if (m_copy != null)
            {
                MP_Copy[] vm_copy = new MP_Copy[k_copy + 1];
                for (int i = 0; i < k_copy; i++)
                    vm_copy[i] = m_copy[i];
                m_copy = vm_copy;
            }
            else
                m_copy = new MP_Copy[1];

            MP_Copy copy = new MP_Copy(emitter);
            m_copy[k_copy] = copy;
            k_copy++;

            return copy;
        }

        // eng: Deleting file with particles copy
        // rus: Удаление файла с копией частиц
        public void DeleteCopy(MP_Copy copy)
        {
            // eng: it is necessary to delete copy
            // rus: надо удалить копию
            int i;

            int index = -1;
            for (i = 0; i < k_copy; i++)
            {
                if (m_copy[i] == copy)
                {
                    index = i;
                    break;
                }
            }

            m_copy[index].Destroy();

            if (k_copy == 1)
            {
                m_copy = null;
            }
            else
            {
                MP_Copy[] vm_copy = new MP_Copy[k_copy - 1];
                for (i = 0; i < index; i++)
                    vm_copy[i] = m_copy[i];

                for (i = index + 1; i < k_copy; i++)
                    vm_copy[i - 1] = m_copy[i];
                m_copy = vm_copy;
            }

            k_copy--;
        }

        // eng: Searching among files containing particle copies by specified emitter id
        // rus: Поиск среди файлов копий частиц соответствующего указанному идентификатору эмиттера
        public MP_Copy FindCopy(uint emitter_id)
        {
            if (CopyMode)
            {
                for (int i = 0; i < k_copy; i++)
                {
                    MP_Copy copy = m_copy[i];
                    if (copy.EmitterID == emitter_id)
                        return copy;
                }
            }

            return null;
        }
    
        // rus: Возвращает дескриптор препядствия для переданного collider'a (создает, если не существовало)
        public unsafe int GetObstacle (Collider collider)
        {
            if (collider == null)
                return 0;

            // rus: проверить существование препятствия
            if (m_obstacles.Contains(collider))
                return (int) m_obstacles[collider];

            // rus: формирование информации и треугольников описывающих препятствие
            MP_Core.MAGIC_POSITION pos = collider.transform.position;
            if (!Magic.CreateTriangles(collider, ref obstacle, ref triangles))
                return 0;

            // rus: регистрировать препятствие в api
            fixed (MP_Core.MAGIC_TRIANGLE* pTr = triangles)
            {
                obstacle.primitives = pTr;

                int hmObstacle = MP_Core.Magic_CreateObstacle(ref obstacle, ref pos, 0);
                m_obstacles[collider] = hmObstacle;

                return hmObstacle;
            }
        }

        // rus: Обновляет сведения о препятствии для collider'a в api
        public unsafe bool UpdateObstacle(Collider collider, int hObstacle=0)
        {
            if (collider == null)
                return false;

            int hmObstacle = hObstacle;

            // rus: получить дескриптор препядствия среди созданных ранее
            if (hObstacle == 0)
            {
                if (!m_obstacles.Contains(collider))
                    return false;

                hmObstacle = (int)m_obstacles[collider];                
            }

            // rus: формирование информации и треугольников описывающих препятствие                        
            if (!Magic.CreateTriangles(collider, ref obstacle, ref triangles))
                return false;

            // rus: обновить препятствие в api
            fixed (MP_Core.MAGIC_TRIANGLE* pTr = triangles)
            {
                obstacle.primitives = pTr;
                return MP_Core.Magic_SetObstacleData(hmObstacle, ref obstacle, 0) == MP_Core.MAGIC_SUCCESS;
            }
        }

        // eng: Deletes obstacle
        // rus: Уничтожает препятствие
        public void RemoveObstacle (Collider collider)
        {
            if (m_obstacles.Contains(collider))
            {
                int hm = (int)m_obstacles[collider];
                m_obstacles.Remove(collider);

                MP_Core.Magic_DestroyPhysicObject(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_OBSTACLE, hm);
            }
        }

        // eng: Deletes all obstacles
        // rus: Уничтожает все препятствия
        public void ClearObjstacles ()
        {
            m_obstacles.Clear();
            MP_Core.Magic_DestroyAllPhysicObjects(MP_Core.MAGIC_PHYSIC_TYPE_ENUM.MAGIC_TYPE_OBSTACLE);
        }
    };

}