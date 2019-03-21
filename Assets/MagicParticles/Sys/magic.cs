//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Sedov Alexey & Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    public class MP_Core
    {
        public static string VersionDLL = "Unknown";

        public const double MAGIC_PI = 3.1415926535897932384626433832795028841971693993751058209;

        public const int MAGIC_SUCCESS = -1;
        public const int MAGIC_ERROR = -2;
        public const int MAGIC_UNKNOWN = -3;

        // eng: interpolation mode is taken from emitter settings
        // rus: режим интерполяции берется из эмиттера
        public const int MAGIC_INTERPOLATION_DEFAULT = 0;

        // eng: interpolation is always enabled 
        // rus: всегда использовать интерполяцию
        public const int MAGIC_INTERPOLATION_ENABLE = 1;

        // eng: interpolation is always disabled
        // rus: всегда отключать интерполяцию
        public const int MAGIC_INTERPOLATION_DISABLE = 2;

        // eng: preserve particle positions while changing emitter position or direction 
        // rus: при изменении позиции или направления эмиттера частицы остаются на прежнем месте
        public const int MAGIC_CHANGE_EMITTER_ONLY = 0;

        // eng: move all the special effect when changing emitter position or direction 
        // rus: при изменении позиции или направления эмиттера весь спецэффект перемещается целиком
        public const int MAGIC_CHANGE_EMITTER_AND_PARTICLES = 1;

        // eng: for each special effect setting specified in editor is used 
        // rus: не изменять настройку по умолчанию
        public const int MAGIC_CHANGE_EMITTER_DEFAULT = 2;

        // eng: MAGIC_POSITION - structure to define coordinates
        // rus: MAGIC_POSITION - структура для хранения координат
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_POSITION
        {
            public float x;
            public float y;
#if !MAGIC_2D
            public float z;

            public MAGIC_POSITION(float _x = 0.0f, float _y = 0.0f, float _z = 0.0f)
            {
                x = _x; y = _y; z = _z;
            }

            public static implicit operator Vector3(MAGIC_POSITION pos1)
            {
                return new Vector3(pos1.x, pos1.y, pos1.z);
            }

            public static implicit operator MAGIC_POSITION(Vector3 pos1)
            {
                return new MAGIC_POSITION(pos1.x, pos1.y, pos1.z);
            }

            public static implicit operator Vector2(MAGIC_POSITION pos1)
            {
                return new Vector2(pos1.x, pos1.y);
            }

            public static implicit operator MAGIC_POSITION(Vector2 pos1)
            {
                return new MAGIC_POSITION(pos1.x, pos1.y);
            }
#else
            public MAGIC_POSITION (float _x=0.0f, float _y=0.0f)
            {
                x = _x; y = _y;
            }
#endif
        };

        // eng: MAGIC_DIRECTION - structure to define direction
        // rus: MAGIC_DIRECTION - структура для хранения направления
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_DIRECTION
        {
#if !MAGIC_2D
            public float x, y, z, w;
#else
		    public float angle;
#endif
        };

        // eng: MAGIC_FIND_DATA - structure that is used in searching emitters and directories
        // rus: MAGIC_FIND_DATA - cтруктура для перебора папок и эмиттеров в текущей папке Explicit
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MAGIC_FIND_DATA
        {
            // eng: result
            // rus: результат		                
            public int type;
            public sbyte* name;
            public int animate;

            public int mode;

            // eng: additional data
            // rus: дополнительные данные		    			            
            public void* folder;
            public int index;
        };

        public const int MAGIC_FOLDER = 1;
        public const int MAGIC_EMITTER = 2;

        // eng: MAGIC_PARTICLE - particle structure, containing its properties
        // rus: MAGIC_PARTICLE - структура частицы, описывающая ее характеристики.
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_PARTICLE
        {
            public MAGIC_POSITION position;
            public float size;
            public float size_factor;
            public float angle;
            public uint color;
            public uint frame;
        };

        // eng: MAGIC_TEXTURE - structure, containing texture frame-file information
        // rus: MAGIC_TEXTURE - структура, хранящая информацию о текстуре
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MAGIC_TEXTURE
        {
            public uint length;
            public sbyte* data;
            public int crc;
            public sbyte* file;
            public sbyte* path;

            public float left, top, right, bottom;

            public int frame_width;
            public int frame_height;

            public int texture_width;
            public int texture_height;

            public float pivot_x;
            public float pivot_y;

            public float scale;

            public int source_frame_width;
            public int source_frame_height;
            public int optimized_frame_x;
            public int optimized_frame_y;
            public int optimized_frame_width;
            public int optimized_frame_height;

            // -------------------------------
            public uint id;
            public int type;

            public void* material_properties;
            public void* texture_properties;

            public float material_source_frame_width;
            public float material_source_frame_height;

            public int optimized_frame_x_const;
            public int optimized_frame_y_const;
            public int optimized_frame_width_const;
            public int optimized_frame_height_const;
        };


#if UNITY_IPHONE 
	    // eng: MAGIC_ATLAS - structure, containing information on frame file locations within the textural atlas
	    // rus: MAGIC_ATLAS - структура, хранящая информацию о расположении файл-кадров на текстурном атласе
	    [StructLayout(LayoutKind.Sequential)]
	    public unsafe struct MAGIC_ATLAS
	    {
		    public int width;
		    public int height;
		    public int count;
            public MAGIC_TEXTURE** textures;
	    };
#endif

        // eng: MAGIC_STATIC_ATLAS - structure, containing information of static textural atlas
        // rus: MAGIC_STATIC_ATLAS - структура, хранящая информацию о статическом текстурном атласе
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MAGIC_STATIC_ATLAS
        {
            public sbyte* file;
            public sbyte* path;
            public int width, height;
            public uint ptc_id;
        };

        public enum MAGIC_CHANGE_ATLAS_ENUM { MAGIC_CHANGE_ATLAS_CREATE, MAGIC_CHANGE_ATLAS_DELETE, MAGIC_CHANGE_ATLAS_LOAD, MAGIC_CHANGE_ATLAS_CLEAN, MAGIC_CHANGE_ATLAS__MAX };

        // eng: MAGIC_CHANGE_ATLAS - structure, containing information on changes in textural atlas
        // rus: MAGIC_CHANGE_ATLAS - структура, хранящая информацию об изменении в текстурном атласе
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_CHANGE_ATLAS
        {
            public MAGIC_CHANGE_ATLAS_ENUM type;
            public int index;
            public int emitter;
            public int x, y;
            public int width, height;
            private IntPtr file;
            private IntPtr path;
            public uint length;
            public IntPtr data;
            public uint ptc_id;

            public string File()
            {
                return Marshal.PtrToStringAnsi(file);
            }

            public string Path()
            {
                return Marshal.PtrToStringAnsi(path);
            }
        };

        // eng: MAGIC_KEY - structure, that allows modifying keys on a Timeline
        // rus: MAGIC_KEY - структура, позволяющая модифицировать ключи на Шкале времени
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_KEY
        {
            public double time;

            public MAGIC_POSITION position;
            public MAGIC_POSITION s1, s2;

            public float scale;

            public int number;
            public MAGIC_DIRECTION direction;

            public float opacity;
        };
        public enum MAGIC_KEY_ENUM { MAGIC_KEY_POSITION, MAGIC_KEY_SCALE, MAGIC_KEY_DIRECTION, MAGIC_KEY_OPACITY, MAGIC_KEY__MAX };
        public enum MAGIC_CAMERA_ENUM { MAGIC_CAMERA_FREE, MAGIC_CAMERA_PERSPECTIVE, MAGIC_CAMERA_ORTHO, MAGIC_CAMERA__MAX };

        // eng: MAGIC_CAMERA - structure describing the camera
        // rus: MAGIC_CAMERA - структура, описывающая камеру
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_CAMERA
        {
            public MAGIC_CAMERA_ENUM mode;
            public MAGIC_POSITION pos;
            public MAGIC_POSITION dir;
        };

        // eng: MAGIC_VIEW - structure which contains all settings for Projection Matrix and View Matrix
        // rus: MAGIC_VIEW - структура, описывающая все настройки для матрицы проекции и матрицы вида
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_VIEW
        {
            public MAGIC_POSITION pos;
            public MAGIC_POSITION dir;
            public MAGIC_POSITION up;
            public float fov;
            public float aspect_ratio;
            public float znear;
            public float zfar;
            public int viewport_width;
            public int viewport_height;
        };

        public enum MAGIC_TAIL_ENUM { MAGIC_TAIL_EXISTING_PARTICLES_TAIL, MAGIC_TAIL_EXISTING_PARTICLES_DESTROY, MAGIC_TAIL_EXISTING_PARTICLES_MOVE, MAGIC_TAIL_EXISTING_PARTICLES_NOMOVE, MAGIC_TAIL_EXISTING_PARTICLES_MOVE_DEFAULT, MAGIC_TAIL_EXISTING_PARTICLES__MAX };

        // eng: MAGIC_TAIL - structure for constructing "tail" from particles
        // rus: MAGIC_TAIL - структура для построения "хвоста" из частиц
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_TAIL
        {
            public MAGIC_TAIL_ENUM existing_particles;
            public float factor;
            public int count;
            public float distance;
            public float step;
            public bool rand;
            public bool single_source;
            public bool direction;
            public float animation_position;
            public float over_life1, over_life2;
            public float size1, size2;
            public bool emitter_end;
        };

        // eng: MAGIC_BBOX - structure to define Bounds Box
        // rus: MAGIC_BBOX - структура для хранения Bounds Box
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_BBOX
        {
            public MAGIC_POSITION corner1;
            public MAGIC_POSITION corner2;
        };

        // eng: MAGIC_LIMITER - structure to restrict the region of "creating" of new particles
        // rus: MAGIC_LIMITER - структура для ограничения области "рождения" новых частиц
        public enum MAGIC_LIMITER_ENUM { MAGIC_LIMITER_DISABLED, MAGIC_LIMITER_RECTANGLE, MAGIC_LIMITER_CIRCLE, MAGIC_LIMITER_PARALLELEPIPED, MAGIC_LIMITER_SPHERE, MAGIC_LIMITER__MAX };

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_LIMITER
        {
            public MAGIC_LIMITER_ENUM type;

            public MAGIC_POSITION position;
            public MAGIC_POSITION size;
            public float radius;
        };

        // eng: MAGIC_ORIENTATION - structure for storing particles type orientation
        // rus: MAGIC_ORIENTATION - структура для храниения ориентации типа частиц
        public enum MAGIC_ORIENTATION_ENUM { MAGIC_ORIENTATION_X, MAGIC_ORIENTATION_Y, MAGIC_ORIENTATION_Z, MAGIC_ORIENTATION_AXIS, MAGIC_ORIENTATION_CAMERA, MAGIC_ORIENTATION_CAMERA_X, MAGIC_ORIENTATION_CAMERA_Y, MAGIC_ORIENTATION_CAMERA_Z, MAGIC_ORIENTATION_CAMERA_AXIS, MAGIC_ORIENTATION_DIRECTION, MAGIC_ORIENTATION__MAX };

        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_ORIENTATION
        {
            public MAGIC_ORIENTATION_ENUM orientation;
            public float x, y, z;
        };

        // eng: MAGIC_VARIABLE - structure for variables
        // rus: MAGIC_VARIABLE - структура для переменных
        public enum MAGIC_VARIABLE_ENUM { MAGIC_VARIABLE_BOOL, MAGIC_VARIABLE_INT, MAGIC_VARIABLE_FLOAT, MAGIC_VARIABLE_STRING, MAGIC_VARIABLE_DIAGRAM, MAGIC_VARIABLE__MAX };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct MAGIC_VARIABLE
        {
            private IntPtr name;
            public MAGIC_VARIABLE_ENUM type;
            public fixed sbyte value[8];

            public string Name()
            {
                return Marshal.PtrToStringAnsi(name);
            }
        };

        // eng: MAGIC_TRIANGLE - structure to define a triangle
        // rus: MAGIC_TRIANGLE - структура для хранения треугольника
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_TRIANGLE
        {
            public MAGIC_POSITION vertex1;
            public MAGIC_POSITION vertex2;
            public MAGIC_POSITION vertex3;
        };

        // eng: MAGIC_WIND - structure that defines wind
        // rus: MAGIC_WIND - структура, которая описывает ветер
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_WIND
        {
            public MAGIC_POSITION direction;
            public float velocity;
        };

        // eng: MAGIC_SEGMENT - structure to store line segment coordinates.
        // rus: MAGIC_SEGMENT - структура для хранения координат отрезка.
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_SEGMENT
        {
            public MAGIC_POSITION vertex1;
            public MAGIC_POSITION vertex2;
        };

        // eng: MAGIC_RECT - structure to store rectangle
        // rus: MAGIC_RECT - структура для хранения прямоугольника
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        };

        // eng: MAGIC_ACTION - structure for actions
        // rus: MAGIC_ACTION - структура для действий
        public enum MAGIC_EVENT_ENUM { MAGIC_EVENT_CREATION, MAGIC_EVENT_DESTRUCTION, MAGIC_EVENT_EXISTENCE, MAGIC_EVENT_COLLISION, MAGIC_EVENT_MAGNET, MAGIC_EVENT_WIND, MAGIC_EVENT__MAX };
        public enum MAGIC_ACTION_ENUM { MAGIC_ACTION_EVENT, MAGIC_ACTION_DESTRUCTION, MAGIC_ACTION_DETACHING, MAGIC_ACTION_FACTOR, MAGIC_ACTION_PARTICLE, MAGIC_ACTION_MAGNET_PARTICLE, MAGIC_ACTION__MAX, MAGIC_ACTION__ERROR = -1 };
        public enum MAGIC_ACTION_DIRECTION_ENUM { MAGIC_ACTION_DIRECTION_DISABLED, MAGIC_ACTION_DIRECTION_MOVEMENT, MAGIC_ACTION_DIRECTION_REBOUND, MAGIC_ACTION_DIRECTION__MAX };
        public enum MAGIC_MAGNET_POINT_ENUM { MAGIC_MAGNET_POINT_ANY, MAGIC_MAGNET_POINT_DIRECTION, MAGIC_MAGNET_POINT_OPPOSITE_DIRECTION, MAGIC_MAGNET_POINT__MAX };

        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_ACTION
        {
            public MAGIC_EVENT_ENUM _event;
            public int HM;
            public MAGIC_ACTION_ENUM action;
            public float factor;
            public int magnet_particles_type;
            public MAGIC_MAGNET_POINT_ENUM magnet_direction;
            public int magnet_distance;
            public float magnet_strength1;
            public float magnet_strength2;

            public int creating_emitter;
            public int particles_type;
            public MAGIC_ACTION_DIRECTION_ENUM direction;
            public float angle;
            public float size;
            public float velocity;
            public float weight;
            public float spin;
            public float angular_velocity;
            public float motion_rand;
            public float visibility;
        };

        // eng: MAGIC_EVENT - structure to get event information
        // rus: MAGIC_EVENT - структура для получения информации о событии
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_EVENT
        {
            public MAGIC_EVENT_ENUM _event;
            public int hmParticle;
            public MAGIC_POSITION position1;
            public MAGIC_POSITION position2;
            public MAGIC_POSITION reflection;
        };

        // eng: MAGIC_OBSTACLE - structure defining the shape of obstacle
        // rus: MAGIC_OBSTACLE - структура, которая описывает форму препятствия
#if !MAGIC_2D
        public enum MAGIC_OBSTACLE_ENUM { MAGIC_OBSTACLE_SPHERE = 3, MAGIC_OBSTACLE_TRIANGLE, MAGIC_OBSTACLE__MAX };
#else
        public enum MAGIC_OBSTACLE_ENUM {MAGIC_OBSTACLE_CIRCLE, MAGIC_OBSTACLE_SEGMENT, MAGIC_OBSTACLE__MAX};
#endif

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MAGIC_OBSTACLE
        {
            public MAGIC_OBSTACLE_ENUM type;
            public float radius;
            public int count;
#if !MAGIC_2D
            public MAGIC_TRIANGLE* primitives;
#else
            public MAGIC_SEGMENT* primitives;
#endif
        };

        // eng: MAGIC_VERTEX_FORMAT - structure that defines format of vertex buffer
        // rus: MAGIC_VERTEX_FORMAT - структура для хранения формата вершинного буфера
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_VERTEX_FORMAT
        {
            public int attributes;
            public int UVs;
        };

        public const int MAGIC_ATTRIBUTE_COLOR = 1;
        public const int MAGIC_ATTRIBUTE_NORMAL = 2;
        public const int MAGIC_ATTRIBUTE_TANGENT = 4;
        public const int MAGIC_ATTRIBUTE_SHADER = 8;

        // eng: MAGIC_RENDERING_START - structure for emitter visualization
        // rus: MAGIC_RENDERING_START - структура для визуализации эмиттера
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_RENDERING_START
        {
            public int particles;				// количество частиц
            public int arrays;					// количество массивов
            public MAGIC_VERTEX_FORMAT format;	// суммарный формат вершин
            public int vertices;				// количество вершин
            public int indexes;				    // количество индексов
            public int textures;				// количество текстур
        };

        public const int MAGIC_BLENDING_NORMAL = 0;
        public const int MAGIC_BLENDING_ADD = 1;
        public const int MAGIC_BLENDING_OPACITY = 2;
        public const int MAGIC_BLENDING_MASK = 3;
        public const int MAGIC_BLENDING_SHADER = 4;

        public const int MAGIC_TEXADDRESS_WRAP = 0;
        public const int MAGIC_TEXADDRESS_MIRROR = 1;
        public const int MAGIC_TEXADDRESS_CLAMP = 2;
        public const int MAGIC_TEXADDRESS_BORDER = 3;

        public const int MAGIC_TEXOP_ARGUMENT1 = 0;
        public const int MAGIC_TEXOP_ADD = 1;
        public const int MAGIC_TEXOP_SUBTRACT = 2;
        public const int MAGIC_TEXOP_MODULATE = 3;
        public const int MAGIC_TEXOP_MODULATE2X = 4;
        public const int MAGIC_TEXOP_MODULATE4X = 5;

        public const int MAGIC_TEXARG_CURRENT = 0;
        public const int MAGIC_TEXARG_DIFFUSE = 1;
        public const int MAGIC_TEXARG_TEXTURE = 2;


        // eng: MAGIC_ARRAY_INFO - structure for description of one array of attribute
        // rus: MAGIC_ARRAY_INFO - структура для описания одного массива атрибута
        public enum MAGIC_VERTEX_FORMAT_ENUM { MAGIC_VERTEX_FORMAT_INDEX, MAGIC_VERTEX_FORMAT_POSITION, MAGIC_VERTEX_FORMAT_COLOR, MAGIC_VERTEX_FORMAT_UV, MAGIC_VERTEX_FORMAT_NORMAL, MAGIC_VERTEX_FORMAT_TANGENT, MAGIC_VERTEX_FORMAT_BINORMAL, MAGIC_VERTEX_FORMAT_ALL, MAGIC_VERTEX_FORMAT__MAX };

        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_ARRAY_INFO
        {
            public MAGIC_VERTEX_FORMAT_ENUM type;
            public int index;
            public int length;
            public int bytes_per_one;
            public int locked_start;
            public int locked_length;
        };


        // eng: MAGIC_RENDER_STATE - structure that contains the change of render state
        // rus: MAGIC_RENDER_STATE - структура, содержащая статус изменения рендера.
        public enum MAGIC_RENDER_STATE_ENUM { MAGIC_RENDER_STATE_BLENDING, MAGIC_RENDER_STATE_TEXTURE_COUNT, MAGIC_RENDER_STATE_TEXTURE, MAGIC_RENDER_STATE_ADDRESS_U, MAGIC_RENDER_STATE_ADDRESS_V, MAGIC_RENDER_STATE_OPERATION_RGB, MAGIC_RENDER_STATE_ARGUMENT1_RGB, MAGIC_RENDER_STATE_ARGUMENT2_RGB, MAGIC_RENDER_STATE_OPERATION_ALPHA, MAGIC_RENDER_STATE_ARGUMENT1_ALPHA, MAGIC_RENDER_STATE_ARGUMENT2_ALPHA, MAGIC_RENDER_STATE_ZENABLE, MAGIC_RENDER_STATE_ZWRITE, MAGIC_RENDER_STATE_ALPHATEST_INIT, MAGIC_RENDER_STATE_ALPHATEST, MAGIC_RENDER_STATE_TECHNIQUE_ON, MAGIC_RENDER_STATE_TECHNIQUE_OFF, MAGIC_RENDER_STATE__MAX, MAGIC_RENDER_STATE__ERROR = -1 };

        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_RENDER_STATE
        {
            public MAGIC_RENDER_STATE_ENUM state;		// флаг
            public int value;							// значение
            public int index;							// дополнительный индекс, который обычно определяет текстурный слот
        };

        // eng: MAGIC_RENDER_VERTICES - structure for visualization of one portion of particles
        // rus: MAGIC_RENDER_VERTICES - структура для визуализации одной порции частиц
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_RENDER_VERTICES
        {
            public int starting_index;
            public int indexes_count;
            public int material;
        };

        // eng: MAGIC_TEXTURE_STATES - structure that describes texture parameters
        // rus: MAGIC_TEXTURE_STATES - структура, описывающая параметры текстуры
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGIC_TEXTURE_STATES
        {
            public int address_u, address_v;				// адресация текстуры по U и V
            public int operation_rgb;						// текстурная операция rgb
            public int argument_rgb1, argument_rgb2;		// аргументы текстурной операции rgb
            public int operation_alpha;					    // текстурная операция alpha
            public int argument_alpha1, argument_alpha2;	// аргументы текстурной операции alpha
        };

        // eng: MAGIC_MATERIAL - structure that describes the material
        // rus: MAGIC_MATERIAL - структура, описывающая параметры материала
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MAGIC_MATERIAL
        {
            public int blending;

            public int textures;
            public MAGIC_TEXTURE_STATES* states;

            public int flags;

            public MAGIC_VERTEX_FORMAT format;
        };

        public const int MAGIC_MATERIAL_ALPHATEST = 1;
        public const int MAGIC_MATERIAL_ZENABLE = 2;
        public const int MAGIC_MATERIAL_ZWRITE = 4;

        // --------------------------------------------------------------------------------------
        // eng: Returns the version of API
        // rus: Возвращает версию API
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_GetVersion();

        // eng: Returns the camera that is set for API
        // rus: Возвращает камеру, установленную для API
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_GetCamera(ref MAGIC_CAMERA camera);

        // eng: Sets the camera for API
        // rus: Устанавливает камеру для API
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_SetCamera(ref MAGIC_CAMERA camera);

#if !MAGIC_2D
        public enum MAGIC_AXIS_ENUM { MAGIC_pXpYpZ, MAGIC_pYpXpZ, MAGIC_pZpXpY, MAGIC_pXpZpY, MAGIC_pYpZpX, MAGIC_pZpYpX, MAGIC_nXpYpZ, MAGIC_pYnXpZ, MAGIC_pZnXpY, MAGIC_nXpZpY, MAGIC_pYpZnX, MAGIC_pZpYnX, MAGIC_pXnYpZ, MAGIC_nYpXpZ, MAGIC_pZpXnY, MAGIC_pXpZnY, MAGIC_nYpZpX, MAGIC_pZnYpX, MAGIC_pXpYnZ, MAGIC_pYpXnZ, MAGIC_nZpXpY, MAGIC_pXnZpY, MAGIC_pYnZpX, MAGIC_nZpYpX, MAGIC_nXnYpZ, MAGIC_nYnXpZ, MAGIC_pZnXnY, MAGIC_nXpZnY, MAGIC_nYpZnX, MAGIC_pZnYnX, MAGIC_nXpYnZ, MAGIC_pYnXnZ, MAGIC_nZnXpY, MAGIC_nXnZpY, MAGIC_pYnZnX, MAGIC_nZpYnX, MAGIC_pXnYnZ, MAGIC_nYpXnZ, MAGIC_nZpXnY, MAGIC_pXnZnY, MAGIC_nYnZpX, MAGIC_nZnYpX, MAGIC_nXnYnZ, MAGIC_nYnXnZ, MAGIC_nZnXnY, MAGIC_nXnZnY, MAGIC_nYnZnX, MAGIC_nZnYnX, MAGIC_AXIS__MAX };
#else
        public enum MAGIC_AXIS_ENUM {MAGIC_pXpY, MAGIC_pYpX, MAGIC_nXpY, MAGIC_pYnX, MAGIC_pXnY, MAGIC_nYpX, MAGIC_nXnY, MAGIC_nYnX, MAGIC_AXIS__MAX};
#endif

        // eng: Returns the direction of coordinate axes
        // rus: Возвращает направление координатных осей
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_AXIS_ENUM Magic_GetAxis();

        // eng: Sets the direction of coordinate axes
        // rus: Устанавливает направление координатных осей
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetAxis(MAGIC_AXIS_ENUM axis_index);

        // eng: Returns the flag of the usage of Z-buffer during the visualization
        // rus: Возвращает признак использования Z-buffer-а во время визуализации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsZBufferEnabled();

        // eng: Enables/Disables the usage of Z-buffer during the visualization
        // rus: Разрешает/запрещает использование Z-buffer-а во время визуализации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_EnableZBuffer(bool enable);

        // eng: Sets filter on the states of render changes
        // rus: Устанавливает фильтр на статусы изменения рендера
        // https://msdn.microsoft.com/library/z6cfh6e6(v=vs.90).aspx
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void Magic_SetRenderStateFilter(byte[] filters, bool optimization);

        /*
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport ("__Internal")]
        #else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
        #endif
        public static extern void Magic_SetRenderStateFilter(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeConst=(int) MAGIC_RENDER_STATE_ENUM.MAGIC_RENDER_STATE__MAX)] bool[] filters, 
            bool optimization);
         */

        // eng: Returns the number of materials
        // rus: Возвращает количество материалов
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetMaterialCount();

        // eng: Returns the info about material
        // rus: Возвращает информацию о материале
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetMaterial(int index, ref MAGIC_MATERIAL material);

        // eng: Converts UTF8 string into UTF16
        // rus: Конвертирует строку типа UTF8 в строку типа UTF16
        //ushort[] Magic_UTF8to16(wchar[] str);

        // eng: Converts UTF8 string into UTF32
        // rus: Конвертирует строку типа UTF8 в строку типа UTF32
        //uint* Magic_UTF8to32(wchar[] str);

        // eng: Converts UTF16 string into UTF8
        // rus: Конвертирует строку типа UTF16 в строку типа UTF8
        //wchar[] Magic_UTF16to8(ushort[] str);

        // eng: Converts UTF16 string into UTF32
        // rus: Конвертирует строку типа UTF16 в строку типа UTF32
        //uint[] Magic_UTF16to32(ushort[] str);

        // eng: Converts UTF32 string into UTF8
        // rus: Конвертирует строку типа UTF32 в строку типа UTF8
        //wchar[] Magic_UTF32to8(uint[] str);

        // eng: Converts UTF32 string into UTF16
        // rus: Конвертирует строку типа UTF32 в строку типа UTF16
        /*ushort[] Magic_UTF32to16(uint[] str);*/

        // eng: Populates MAGIC_ACTION structure with default values
        // rus: Заполняет структуру MAGIC_ACTION значениями по умолчанию
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void MAGIC_ACTION_Identity(ref MAGIC_ACTION action);

        // --------------------------------------------------------------------------------------

        // eng: Loads the ptc-file from the path specified
        // rus: Загружает ptc-файл по указанному пути
        //int Magic_OpenFile(char[] file_name);		 

        // eng: Loads the ptc-file image from the memory
        // rus: Открытие образа ptc-файла из памяти
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_OpenFileInMemory(byte[] buffer);

        // eng: Loads the ptc-file from the stream
        // rus: Загружает ptc-файл из указанного потока
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_OpenStream(int hmStream);

        // eng: Closes the file, opened earlier by use of Magic_OpenFile, Magic_OpenFileInMemory or Magic_OpenStream
        // rus: Закрывает файл, открытый ранее через Magic_OpenFile, Magic_OpenFileInMemory или Magic_OpenStream
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_CloseFile(int hmFile);

        // eng: Closing all the opened files
        // rus: Закрытие всех открытых файлов
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_CloseAllFiles();

        // eng: Returns the current folder path
        // rus: Возвращает полный путь к текущей папке
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_GetCurrentFolder(int hmFile);

        // eng: Sets the new current folder
        // rus: Установить новый путь к текущей папке
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_SetCurrentFolder(int hmFile, string path);

        // eng: Searches for the first folder or emitter within the current folder and returns the type of the object found
        // rus: Ищет первую папку или первый эмиттер в текущей папке и возвращает имя и тип найденного объекта
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_FindFirst(int hmFile, ref MAGIC_FIND_DATA data, int mode);

        // eng: Searches for the next folder or emitter within the current folder and returns the type of the object found
        // rus: Ищет очередную папку или очередной эмиттер в текущей папке и возвращает имя и тип найденного объекта
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_FindNext(int hmFile, ref MAGIC_FIND_DATA data);

        // eng: Returns the name of the file that was opened through the Magic_OpenFile
        // rus: Возвращает имя файла, открытого через Magic_OpenFile
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_GetFileName(int hmFile);

        // eng: Returns the flag indicating that textures are stored within the file
        // rus: Возвращает признак того, что файл содержит текстуры
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_HasTextures(int hmFile);

        // eng: Returns the number of static textural atlases attached to specified file
        // rus: Возвращает количество статических текстурных атласов, прикрепленных к указанному файлу
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetStaticAtlasCount(int hmFile);

        // eng: Returns information on static textural atlas attached to specified file
        // rus: Возвращает информацию о статическом текстурном атласе, прикрепленному к указанному файлу
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetStaticAtlas(int hmFile, int index, ref MAGIC_STATIC_ATLAS atlas);

        // --------------------------------------------------------------------------------

        // eng: Creates the emitter object and loads its data
        // rus: Создает эмиттер и загружает в него данные
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_LoadEmitter(int hmFile, string name);

        // eng: Gets the copy of the emitter
        // rus: Дублирует эмиттер
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DuplicateEmitter(int hmEmitter);

        // eng: Unloads the emitter data and destroys it
        // rus: Выгружает данные из эмиттера и уничтожает его
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_UnloadEmitter(int hmEitter);

        // eng: Unloads all loaded emitters
        // rus: Выгружает все загруженные эмиттеры
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_UnloadAllEmitters();

        // eng: Processes the emitter. Creates, displaces and removes the particles
        // rus: Осуществляет обработку эмиттера: создает, перемещает и уничтожает частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_Update(int hmEmitter, double time);

        // eng: Stops the emitter
        // rus: Останавливает работу эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_Stop(int hmEmitter);

        // eng: Restarts the emitter from the beginning
        // rus: Перезапускает эмиттер с нулевой позиции
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_Restart(int hmEmitter);

        // eng: Returns the flag showing that emitter is in interrupted mode
        // rus: Возврашает признак того, что эмиттер прерывается
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsInterrupt(int hmEmitter);

        // eng: Interrupts/Starts emitter work
        // rus: Прерывает или запускает работу эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetInterrupt(int hmEmitter, bool interrupt);

        // eng: Returns the Magic Particles (Dev) time increment, used for the animation
        // rus: Возвращает заданное в Magic Particles приращение времени, используемое для анимации эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern double Magic_GetUpdateTime(int hmEmitter);

        // eng: Returns current animation position
        // rus: Возвращает текущую позицию анимации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern double Magic_GetPosition(int hmEmitter);

        // eng: Sets the current animation position
        // rus: Устанавливает текущую позицию анимации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetPosition(int hmEmitter, double position);

        // eng: Returns animation duration
        // rus: Возвращает продолжительность анимации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern double Magic_GetDuration(int hmEmitter);

        // eng: Returns the left position of the visibility range
        // rus: Возвращает левую позицию интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern double Magic_GetInterval1(int hmEmitter);

        // eng: Sets the left position of the visibility range
        // rus: Устанавливает левую позицию интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetInterval1(int hmEmitter, double position);

        // eng: Returns the right position of the visibility range
        // rus: Возвращает правую позицию интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern double Magic_GetInterval2(int hmEmitter);

        // eng: Sets the right position of the visibility range
        // rus: Устанавливает правую позицию интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetInterval2(int hmEmitter, double position);

        // eng: Figures out if the current animation position is within the visibility range
        // rus: Определяет, попадает ли текущая позиция анимации в интервал видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_InInterval(int hmEmitter);

        // eng: Sets the animation position at the left position of visibility range
        // rus: Устанавливает эмиттер на первую границу интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_EmitterToInterval1(int hmEmitter, float speed_factor, string file);

        // eng: Returns the flag of the animation of emitter that begins not from 0 position
        // rus: Возвращает признак того, что анимация эмиттера начинается не с начала
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsInterval1(int hmEmitter);

        // eng: Copying the particles array into emitter from the file
        // rus: Копирование пространства частиц в эмиттер из файла.
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_LoadArrayFromFile(int hmEmitter, string file);

        // eng: Copying the particles array from the emitter into the file
        // rus: Копирование пространства частиц эмиттера в файл
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_SaveArrayToFile(int hmEmitter, string file);

        // eng: Returns the particle positions interpolation usage flag
        // rus: Возвращает признак режима интерполяции эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsInterpolationMode(int hmEmitter);

        // eng: Sets/resets the particle positions interpolation usage flag
        // rus: Включает/отключает режим интреполяции положения частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetInterpolationMode(int hmEmitter, bool mode);

        // eng: Returns the flag of stability/randomness of the emitter behaviour
        // rus: Возвращает признак стабильности/случайности поведения эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsRandomMode(int hmEmitter);

        // eng: Sets the flag of stability/randomness of the emitter behaviour
        // rus: Устанавливает признак стабильности/случайности поведения эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetRandomMode(int hmEmitter, bool mode);

        public enum MAGIC_LOOP_ENUM { MAGIC_NOLOOP, MAGIC_LOOP, MAGIC_FOREVER, MAGIC_LOOP__MAX, MAGIC_LOOP__ERROR = MAGIC_ERROR };
        // eng: Returns the emitter behaviour mode at the end of the animation
        // rus: Возвращает режим поведения эмиттера после окончания анимации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_LOOP_ENUM Magic_GetLoopMode(int hmEmitter);

        // eng: Sets the emitter behaviour mode at the end of the animation
        // rus: Устанавливает режим поведения эмиттера после окончания анимации
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetLoopMode(int hmEmitter, MAGIC_LOOP_ENUM mode);

        public enum MAGIC_COLOR_ENUM { MAGIC_COLOR_STANDARD, MAGIC_COLOR_TINT, MAGIC_COLOR_USER, MAGIC_COLOR__MAX, MAGIC_COLOR__ERROR = MAGIC_ERROR };
        // eng: Returns the color management mode
        // rus: Возвращает режим управления цветом частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_COLOR_ENUM Magic_GetColorMode(int hmEmitter);

        // eng: Sets the color management mode
        // rus: Устанавливает режим управления цветом частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetColorMode(int hmEmitter, MAGIC_COLOR_ENUM mode);

        // eng: Returns the user defined tint
        // rus: Возвращает оттенок пользователя
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetTint(int hmEmitter);

        // eng: Sets the user defined tint
        // rus: Устанавливает оттенок пользователя
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetTint(int hmEmitter, int tint);

        // eng: Returns the user defined tint strength
        // rus: Возвращает силу оттенка пользователя
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetTintStrength(int hmEmitter);

        // eng: Sets the user defined tint strength
        // rus: Устанавливает силу оттенка пользователя
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetTintStrength(int hmEmitter, float tint_strength);

        // eng: Returns the emitter scale
        // rus: Возвращает масштаб эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetScale(int hmEmitter);

        // eng: Sets the emitter scale
        // rus: Устанавливает масштаб эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetScale(int hmEmitter, float scale);

        // eng: Sets the user data
        // rus: Устанавливает пользовательские данные
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetData(int hmEmitter, int data);

        // eng: Returns the user data
        // rus: Возвращает пользовательские данные
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetData(int hmEmitter);

        // rus: Возвращает область рождения частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetLimiter(int hmEmitter, ref MAGIC_LIMITER place);

        // rus: Устанавливает область рождения частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetLimiter(int hmEmitter, ref MAGIC_LIMITER place);

        // eng: Returns the name of the emitter
        // rus: Возвращает имя эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_GetEmitterName(int hmEmitter);

        public enum MAGIC_EMITTER_ENUM { MAGIC_EMITTER_POINT, MAGIC_EMITTER_LINE, MAGIC_EMITTER_CIRCLE, MAGIC_EMITTER_ELLIPSE, MAGIC_EMITTER_SQUARE, MAGIC_EMITTER_RECTANGLE, MAGIC_EMITTER_IMAGE, MAGIC_EMITTER_TEXT, MAGIC_EMITTER_MODEL, MAGIC_EMITTER__MAX, MAGIC_EMITTER__ERROR = MAGIC_ERROR };
        // eng: Returns the shape of the emitter itself or the shape of the emitter for the specified particles type
        // rus: Возвращает форму эмиттера или форму эмиттера для указанного типа частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_EMITTER_ENUM Magic_GetEmitterType(int hmEmitter, int index);

        // eng: Returns the mode of the emitter
        // rus: Возвращает режим работы эмиттера
        public enum MAGIC_MODE_ENUM { MAGIC_MODE_FOLDER, MAGIC_MODE_EMITTER, MAGIC_MODE_PICTURE, MAGIC_MODE_TEXT, MAGIC_MODE_MODEL, MAGIC_MODE__MAX, MAGIC_MODE__ERROR = MAGIC_ERROR };
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_MODE_ENUM Magic_GetEmitterMode(int hmEmitter);

        // eng: Returns the ID of emitter
        // rus: Возвращает идентификатор эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern uint Magic_GetEmitterID(int hmEmitter);

        // eng: Returns the value of "Speed" that was set in Magic Particles (Dev)
        // rus: Возвращает заданное в Magic Particles (Dev) значение "коэффициент темпа"
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetUpdateSpeed(int hmEmitter);

        // eng: Returns information about background image of emitter
        // rus: Возвращает информацию о фоновом изображении эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetBackgroundRect(int hmEmitter, ref MAGIC_RECT rect);

#if !MAGIC_2D
        // eng: Returns the flag indicating that emitter emits only 3D-particles
        // rus: Возвращает признак того, что эмиттер излучает только 3D-частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_Is3d(int hmEmitter);

        // eng: Returns the default camera and perspective of emitter that were set using the Magic Particles (Dev)
        // rus: Возвращает камеру и перспективу, которые были установлены для эмиттера в Magic Particles (Dev)
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetView(int hmEmitter, ref MAGIC_VIEW view);
#else
		// eng: Transforms coordinates of 2D-emitter from Magic Particles (Dev) into scene coordinates
		// rus: Преобразует координаты 2D-эмиттера из Magic Particles (Dev) в координаты сцены
       
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
		[DllImport("magic")]
#endif
		public static extern int Magic_CorrectEmitterPosition(int hmEmitter, int scene_width, int scene_height);
#endif

        // eng: Returns coordinates of the emitter
        // rus: Возвращает координаты эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitterPosition(int hmEmitter, ref MAGIC_POSITION pos);

        // eng: Sets the coordinates of the emitter
        // rus: Устанавливает координаты эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterPosition(int hmEmitter, ref MAGIC_POSITION pos);

        // eng: Sets the emitter position. "Tail" from particles is formed between old and new position
        // rus: Устанавливает координаты эмиттера. Между старой и новой позицией эмиттера образуется "хвост" из частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterPositionWithTail(int hmEmitter, ref MAGIC_POSITION pos);

        // eng: Sets the coordinates of the emitter (attached physic objects are moved too)
        // rus: Устанавливает координаты эмиттера (происходит перемещение всех прицепленных физ. объектов)
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterPositionWithAttachedPhysicObjects(int hmEmitter, ref MAGIC_POSITION pos);

        // eng: Returns the mode of the emitter coordinates
        // rus: Возвращает режим координат эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_GetEmitterPositionMode(int hmEmitter);

        // eng: Sets the mode of the emitter coordinates
        // rus: Устанавливает режим координат эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterPositionMode(int hmEmitter, bool mode);

        // eng: Moves particles
        // rus: Перемещает частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_MoveEmitterParticles(int hmEmitter, ref MAGIC_POSITION offset);

        // eng: Returns emitter direction
        // rus: Возвращает направление эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitterDirection(int hmEmitter, ref MAGIC_DIRECTION direction);

        // eng: Sets the direction of the emitter
        // rus: Устанавливает направление эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterDirection(int hmEmitter, ref MAGIC_DIRECTION direction);

        // eng: Gets the emitter's direction mode
        // rus: Возвращает режим вращения эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_GetEmitterDirectionMode(int hmEmitter);

        // eng: Sets emitter's rotation mode
        // rus: Устанавливает режим вращения эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetEmitterDirectionMode(int hmEmitter, bool mode);

        // eng: Rotates particles
        // rus: Вращает частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_RotateEmitterParticles(int hmEmitter, ref MAGIC_DIRECTION offset);

        // eng: Returns the animate folder flag
        // rus: Возвращает признак анимированной папки
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsFolder(int hmEmitter);

        // eng: Returns the number of emitters contained in animate folder. 1 is returned for emitter
        // rus: Возвращает количество эмиттеров внутри эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitterCount(int hmEmitter);

        // eng: Returns the specified emitter from animate folder. Returns itself for emitter
        // rus: Возвращает дескриптор эмиттера внутри эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitter(int hmEmitter, int index);

        //-----------------------------------------------------------------------------------------

        // eng: Prepares the information about render arrays
        // rus: Подготавливает информацию о массивах рендера
        public enum MAGIC_ARGB_ENUM { MAGIC_ABGR, MAGIC_ARGB, MAGIC_BGRA, MAGIC_RGBA, MAGIC_ARGB__MAX };
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_PrepareRenderArrays(int hmEmitter, ref MAGIC_RENDERING_START start, int max_arrays_streams, MAGIC_ARGB_ENUM argb_format, bool index32);

        // eng: Returns the info about one of render arrays
        // rus: Возвращает информацию об одном из массивов рендера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal", EntryPoint="Magic_GetRenderArrayData")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_GetRenderArrayData(IntPtr context, int index, ref MAGIC_ARRAY_INFO info);

        // eng: Sets the render array
        // rus: Устанавливает массив рендера
#if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint = "Magic_SetRenderArrayData")]
#else
        [DllImport("magic3d", EntryPoint = "Magic_SetRenderArrayData", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern unsafe int Magic_SetRenderArrayData(IntPtr context, int index, void* buffer, int offset, int stride);
        /*
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint = "Magic_SetRenderArrayData")]
        #else
        [DllImport("magic3d", EntryPoint = "Magic_SetRenderArrayData", CallingConvention = CallingConvention.Cdecl)]
        #endif
        public static extern int Magic_SetRenderArrayDataInt(IntPtr context, int index, int[] buffer, int offset, int stride);
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport ("__Internal", EntryPoint = "Magic_SetRenderArrayData")]
        #else
        [DllImport("magic3d", EntryPoint = "Magic_SetRenderArrayData", CallingConvention = CallingConvention.Cdecl)]
        #endif
        public static extern int Magic_SetRenderArrayDataCol(IntPtr context, int index, Color32[] buffer, int offset, int stride);
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport ("__Internal", EntryPoint = "Magic_SetRenderArrayData")]
        #else
        [DllImport("magic3d", EntryPoint = "Magic_SetRenderArrayData", CallingConvention = CallingConvention.Cdecl)]
        #endif
        public static extern int Magic_SetRenderArrayDataV2(IntPtr context, int index, Vector2[] buffer, int offset, int stride);
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport ("__Internal", EntryPoint = "Magic_SetRenderArrayData")]
        #else
        [DllImport("magic3d", EntryPoint = "Magic_SetRenderArrayData", CallingConvention = CallingConvention.Cdecl)]
        #endif
        public static extern int Magic_SetRenderArrayDataV3(IntPtr context, int index, Vector3[] buffer, int offset, int stride);
        */

        // eng: Fills the render buffers by info about vertices
        // rus: Заполняет буфера визуализации информацией о вершинах
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void Magic_FillRenderArrays(IntPtr context);

        // eng: Returns the portion of particles
        // rus: Возвращает порцию вершин
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_GetVertices(IntPtr context, ref MAGIC_RENDER_VERTICES vrts);

        // eng: Returns a next render state
        // rus: Возвращает очередной статус рендера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_GetNextRenderState(IntPtr context, ref MAGIC_RENDER_STATE state);

        public enum MAGIC_SORT_ENUM { MAGIC_NOSORT, MAGIC_SORT_MIX, MAGIC_SORT_MIX_INV, MAGIC_SORT_CAMERA_NEAR, MAGIC_SORT_CAMERA_FAR, MAGIC_SORT__MAX };
        // eng: Returns particles sorting mode
        // rus: Возвращает режим сортировки частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_SORT_ENUM Magic_GetSortingMode(int hmEmitter);

        // eng: Sets particles sorting mode
        // rus: Устанавливает режим сортировки частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetSortingMode(int hmEmitter, MAGIC_SORT_ENUM mode);

        // eng: Returns maximal Bounding Box
        // rus: Возвращает максимальный Bounding Box
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetBBoxMax(int hmEmitter, ref MAGIC_BBOX bbox);

        // eng: Returnes Bounds Box
        // rus: Возвращает BBox
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetBBox(int hmEmitter, ref MAGIC_BBOX bbox);

        // eng: Returns Bounds Box recalculation period
        // rus: Возвращает период перерасчета Bounds Box
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetBBoxPeriod(int hmEmitter);

        // eng: Sets Bounds Box recalculation period
        // rus: Устанавливает период перерасчета Bounds Box
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetBBoxPeriod(int hmEmitter, int period);

        // eng: Forces Bounds Box recalculation
        // rus: Принудительно пересчитывает Bounds Box
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_RecalcBBox(int hmEmitter);

        // eng: Returns the count of user defined variables of emitter or animated folder
        // rus: Возвращает количество пользовательских переменных внутри эмиттера или анимированной папки
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitterVariableCount(int hmEmitter);

        // eng: Returns information about user defined variable of emitter or animated folder
        // rus: Возвращает информацию о пользовательской переменной из эмиттера или анимированной папки
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetEmitterVariable(int hmEmitter, int index, ref MAGIC_VARIABLE variable);

        // --------------------------------------------------------------------------------
        // eng: Returns the name of the particles type
        // rus: Возвращает имя типа частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_GetParticlesTypeName(int hmEmitter, int index);

        // eng: Returns the number particles type contained in emitter
        // rus: Возвращает количество типов частиц внутри эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetParticlesTypeCount(int hmEmitter);

        // eng: Locks the specified particles type for the further processing
        // rus: Захватывает для дальнейшей обработки указанный тип частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_LockParticlesType(int hmEmitter, int index);

        // eng: Releases previously locked particles type
        // rus: Освобождает захваченный ранее тип частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_UnlockParticlesType();

        // --------------------------------------------------------------------------------
        // eng: Returns particle type orientation for 3D-emitters
        // rus: Возвращает ориентацию типа частиц для 3D-эмиттеров
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetOrientation(ref MAGIC_ORIENTATION orientation);

        // eng: Sets particle type orientation for 3D-emitters
        // rus: Устанавливает ориентацию типа частиц для 3D-эмиттеров
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetOrientation(ref MAGIC_ORIENTATION orientation);
        // --------------------------------------------------------------------------------

        // eng: Returns "tail" properties
        // rus: Возвращает свойства "хвоста"
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetTailProperties(ref MAGIC_TAIL tail);

        // eng: Sets "tail" properties
        // rus: Устанавливает свойства "хвоста"
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetTailProperties(ref MAGIC_TAIL tail);

        // eng: Returns the next particle. Is used to go through all the existing particles
        // rus: Возвращает информацию об очередной частице. Используется для перебора всех существующих частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetNextParticle(ref MAGIC_PARTICLE particle);

        // eng: Changes the position of the particle that is got by Magic_GetNextParticle
        // rus: Изменяет координаты частицы, полученной через Magic_GetNextParticle
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_MoveParticle(ref MAGIC_POSITION offset);

        // eng: Rotates the particle that was obtained by Magic_GetNextParticle around the emitter
        // rus: Вращает частицу полученную через Magic_GetNextParticle вокруг эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_RotateParticle(ref MAGIC_DIRECTION offset);

        // eng: Returns count of user defined variables of particles type
        // rus: Возвращает количество пользовательских переменных внутри типа частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetParticlesTypeVariableCount();

        // eng: Returns information about user defined variable of particles type
        // rus: Возвращает информацию о пользовательской переменной из типа частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetParticlesTypeVariable(int index, ref MAGIC_VARIABLE variable);

        // eng: Returns the count of actions
        // rus: Возвращает количество действий
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetActionCount();

        // eng: Returns MAGIC_VARIABLE structure that contains action information
        // rus: Возвращает структуру MAGIC_ACTION с информацией об указанном действии
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetAction(int index, ref MAGIC_ACTION action);

        // eng: Creates new action
        // rus: Создает новое действие
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_CreateAction(ref MAGIC_ACTION action);

        // eng: Deletes specified action
        // rus: Удаляет указанное действие
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DestroyAction(int index);

        public enum MAGIC_PHYSIC_TYPE_ENUM { MAGIC_TYPE_OBSTACLE, MAGIC_TYPE_WIND, MAGIC_TYPE_MAGNET, MAGIC_PHYSIC_TYPE__MAX };
        // eng: Returns count of attached physical objects of specified type
        // rus: Возвращает количество присоединенных физических объектов указанного типа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetAttachedPhysicObjectsCount(MAGIC_PHYSIC_TYPE_ENUM type);

        // eng: Returns list of attached physical objects of specified type
        // rus: Возвращает список присоединенных физических объектов указанного типа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern unsafe int Magic_GetAttachedPhysicObjects(MAGIC_PHYSIC_TYPE_ENUM type, int* HMs);

        public enum MAGIC_PARTICLES_TYPE_ENUM { MAGIC_PARTICLES_TYPE_USUAL, MAGIC_PARTICLES_TYPE_PATH, MAGIC_PARTICLES_TYPE_BEAM, MAGIC_PARTICLES_TYPE_TRAIL, MAGIC_PARTICLES_TYPE__MAX, MAGIC_PARTICLES_TYPE__ERROR = MAGIC_ERROR };
        // eng: Returns the mode of particles type
        // rus: Возвращает режим работы типа частиц
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_PARTICLES_TYPE_ENUM Magic_GetParticlesTypeMode();

        // eng: Returns the number of keys in the path for Path/Beam
        // rus: Возвращает количество ключей в пути для Path/Beam
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetPathLength();

        // eng: Returns the path for Path/Beam
        // rus: Возвращает путь для Path/Beam
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetPath(ref MAGIC_KEY keys);

        // eng: Changes the path for Path/Beam
        // rus: Изменяет путь для Path/Beam
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetPath(int count, ref MAGIC_KEY keys);

        // --------------------------------------------------------------------------

        // eng: Returns the information on next change in textural atlas
        // rus: Возвращает информацию об очередном изменении в текстурном атласе
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetNextAtlasChange(ref MAGIC_CHANGE_ATLAS change);

        // eng: Creates textural atlases for all loaded emitters
        // rus: Создает текстурные атласы для всех загруженных эмиттеров
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_CreateAtlases(int width, int height, int step, float scale_step);

        // eng: Creates textural atlases for specified emitters
        // rus: Создает текстурные атласы для указанных эмиттеров
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_CreateAtlasesForEmitters(int width, int height, int count, [In] int[] emitters, int step, float scale_step);

        // eng: Sets up the initial scale for atlas creation
        // rus: Устанавливает стартовый масштаб для постороения атласа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_SetStartingScaleForAtlas(float scale);

        // eng: Returns the initial scale for atlas creation
        // rus: Возвращает стартовый масштаб для постороения атласа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetStartingScaleForAtlas();

        // --------------------------------------------------------------------------

        public enum MAGIC_DIAGRAM_ENUM { MAGIC_DIAGRAM_LIFE, MAGIC_DIAGRAM_NUMBER, MAGIC_DIAGRAM_SIZE, MAGIC_DIAGRAM_VELOCITY, MAGIC_DIAGRAM_WEIGHT, MAGIC_DIAGRAM_SPIN, MAGIC_DIAGRAM_ANGULAR_VELOCITY, MAGIC_DIAGRAM_MOTION_RAND, MAGIC_DIAGRAM_VISIBILITY, MAGIC_DIAGRAM_DIRECTION, MAGIC_DIAGRAM__MAX };
        // eng: Figures out if the diagram is managable
        // rus: Определяет, доступен ли указанный график
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern bool Magic_IsDiagramEnabled(int hmEmitter, int type_index, MAGIC_DIAGRAM_ENUM diagram_index);

        // eng: Returns the specified diagram factor
        // rus: Возвращает множитель для указанного графика
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetDiagramFactor(int hmEmitter, int type_index, MAGIC_DIAGRAM_ENUM diagram_index);

        // eng: Sets the specified diagram factor
        // rus: Устанавливает множитель для указанного графика
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetDiagramFactor(int hmEmitter, int type_index, MAGIC_DIAGRAM_ENUM diagram_index, float factor);

        // eng: Returns the factor for emitter form diagram
        // rus: Возвращает множитель для графика формы эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetDiagramEmitterFactor(int hmEmitter, int type_index, bool line);

        // eng: Sets the factor for emitter form diagram
        // rus: Устанавливает множитель для графика формы эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetDiagramEmitterFactor(int hmEmitter, int type_index, bool line, float factor);

        // eng: Returns the offset for the specified diagram
        // rus: Возвращает смещение для указанного графика
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetDiagramAddition(int hmEmitter, int type_index, MAGIC_DIAGRAM_ENUM diagram_index);

        // eng: Sets the offset for the specified diagram
        // rus: Устанавливает смещение для указанного графика
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetDiagramAddition(int hmEmitter, int type_index, MAGIC_DIAGRAM_ENUM diagram_index, float addition);

        // eng: Returns the offset for emitter form diagram
        // rus: Возвращает смещение для графика формы эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_GetDiagramEmitterAddition(int hmEmitter, int type_index, bool line);

        // eng: Sets the offset for emitter form diagram
        // rus: Устанавливает смещение для графика формы эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetDiagramEmitterAddition(int hmEmitter, int type_index, bool line, float addition);

        // eng: Allows substituting a graphical pattern which is used to generate particles of "Image" and "Text" formed emitters
        // rus: Позволяет заменить графический образ, по которому происходит генерация частиц у эмиттеров типа "Картинка" и "Текст"
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern unsafe int Magic_ChangeImage(int hmEmitter, int type_index, int width, int height, void* data, int bytes_per_pixel);

        // eng: Allows changing the triangle based model by which particle generation occurs in "Model" formed emitters
        // rus: Позволяет заменить модель из треугольников, по которой происходит генерация частиц у эмиттера типа "Модель"
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern unsafe int Magic_ChangeModel(int hmEmitter, int type_index, int count, MAGIC_TRIANGLE* triangles);

        // --------------------------------------------------------------------------------------

        // eng: Creates a new key on a Timeline
        // rus: Создает новый ключ указанного типа на Шкале времени
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_CreateKey(int hmEmitter, MAGIC_KEY_ENUM type, ref MAGIC_KEY key);

        // eng: Deletes the specified key of desired type from Timeline
        // rus: Удаляет выбранный ключ указанного типа со Шкалы времени
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DeleteKey(int hmEmitter, MAGIC_KEY_ENUM type, int index);

        // eng: Returns the number of keys of specified type from Timeline
        // rus: Возвращает количество ключей указанного типа на Шкале времени
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetKeyCount(int hmEmitter, MAGIC_KEY_ENUM type);

        // eng: Returns information for the key of specified type
        // rus: Возвращает информацию о ключе указанного типа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetKey(int hmEmitter, MAGIC_KEY_ENUM type, ref MAGIC_KEY key, int index);

        // eng: Sets the new data for the key of specified type
        // rus: Устанавливает новые данные для ключа указанного типа
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetKey(int hmEmitter, MAGIC_KEY_ENUM type, ref MAGIC_KEY key, int index);

        // --------------------------------------------------------------------------------------

        // eng: Creates obstacle
        // rus: Создает препятствие
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_CreateObstacle(ref MAGIC_OBSTACLE data, ref MAGIC_POSITION position, int cell);

        // eng: Returns information about shape of obstacle
        // rus: Возвращает информацию о форме препятствия
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_GetObstacleData(int hmObstacle, ref MAGIC_OBSTACLE data);

        // eng: Sets new shape of obstacle
        // rus: Устанавливает новую форму для препятствия
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_SetObstacleData(int hmObstacle, ref MAGIC_OBSTACLE data, int cell);

        // eng: Returns position of obstacle
        // rus: Возвращает позицию препятствия
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_GetObstaclePosition(int hmObstacle, ref MAGIC_POSITION pos);

        // eng: Sets new position of obstacle
        // rus: Устанавливает новую позицию для препятствия
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_SetObstaclePosition(int hmObstacle, ref MAGIC_POSITION pos);

        // --------------------------------------------------------------------------------------

        // eng: Creates wind
        // rus: Создает ветер
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_CreateWind(ref MAGIC_WIND data);

        // eng: Returns information about wind
        // rus: Возвращает информацию о ветре
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetWindData(int hmWind, ref MAGIC_WIND data);

        // eng: Sets new properties for wind
        // rus: Устанавливает новые свойства для ветра
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_SetWindData(int hmWind, ref MAGIC_WIND data);

        // eng: Deletes obstacle or wind
        // rus: Уничтожает препятствие или ветер
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DestroyPhysicObject(MAGIC_PHYSIC_TYPE_ENUM type, int HM);

        // eng: Deletes all obstacles and winds
        // rus: Уничтожает все препятствия или ветры
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DestroyAllPhysicObjects(MAGIC_PHYSIC_TYPE_ENUM type);

        // eng: Duplicates specified obstacle or wind
        // rus: Снимает копию с указанного препятствия или ветра
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_DuplicatePhysicObject(MAGIC_PHYSIC_TYPE_ENUM type, int HM);

        // --------------------------------------------------------------------------------------

        // eng: Returns information about subsequent event
        // rus: Возвращает информацию об очередном событии
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_GetNextEvent(ref MAGIC_EVENT evt);

        // eng: Returns user data of particle
        // rus: Возвращает пользовательские данные частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern unsafe void* Magic_ParticleGetData(int hmParticle);

        // eng: Sets user data of particle
        // rus: Устанавливает пользовательские данные частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern unsafe void Magic_ParticleSetData(int hmParticle, void* data);

        // eng: Returns scene coordinates of particle
        // rus: Возвращает координаты частицы на сцене
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_ParticleGetPosition(int hmParticle, ref MAGIC_POSITION pos);

        // eng: Sets coordinates of particle
        // rus: Устанавливает координаты частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_ParticleSetPosition(int hmParticle, ref MAGIC_POSITION pos);

        public enum MAGIC_PROPERTY_ENUM { MAGIC_PROPERTY_ANGLE, MAGIC_PROPERTY_SIZE, MAGIC_PROPERTY_VELOCITY, MAGIC_PROPERTY_WEIGHT, MAGIC_PROPERTY_SPIN, MAGIC_PROPERTY_ANGULAR_VELOCITY, MAGIC_PROPERTY_MOTION_RAND, MAGIC_PROPERTY_VISIBILITY, MAGIC_PROPERTY__MAX };

        // eng: Returns specified property of particle
        // rus: Возвращает указанное свойство частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_ParticleGetProperty(int hmParticle, MAGIC_PROPERTY_ENUM property);

        // eng: Sets specified property of particle
        // rus: Устанавливает указанное свойство частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_ParticleSetProperty(int hmParticle, MAGIC_PROPERTY_ENUM property, float value);

        // eng: Returns several specified properties of particle
        // rus: Возвращает несколько указанных свойств частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void Magic_ParticleGetProperties(int hmParticle, int count, int[] properties, float[] values);

        // eng: Sets several specified properties of particle
        // rus: Устанавливает несколько указанных свойств частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void Magic_ParticleSetProperties(int hmParticle, int count, int[] properties, float[] values);

        // eng: Returns physical radius of particle
        // rus: Возвращает физический радиус частицы
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern float Magic_ParticleGetRadius(int hmParticle);

        // eng: Detaches particle from emitter
        // rus: Отсоединяет частицу от эмиттера
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_ParticleDetaching(int hmParticle);

        // eng: Deletes particle
        // rus: Уничтожает частицу
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_ParticleDestruction(int hmParticle);

        // --------------------------------------------------------------------------------------

        public enum MAGIC_STREAM_ENUM { MAGIC_STREAM_READ, MAGIC_STREAM_WRITE, MAGIC_STREAM_ADD, MAGIC_STREAM__MAX, MAGIC_STREAM__ERROR = MAGIC_ERROR };

        // eng: Opens stream from file
        // rus: Открывает поток из файла
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_StreamOpenFile(string file_name, MAGIC_STREAM_ENUM mode);

        // eng: Opens stream in memory
        // rus: Открывает поток в памяти
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_StreamOpenMemory(byte[] address, uint length, MAGIC_STREAM_ENUM mode);

        // eng: Closes stream that was previously opened by Magic_StreamOpenFile or Magic_StreamOpenMemory
        // rus: Закрывает поток, открытый ранее через Magic_StreamOpenFile или Magic_StreamOpenMemory
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_StreamClose(int hmStream);

        // eng: Closing all opened streams
        // rus: Закрытие всех открытых поток
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern void Magic_StreamCloseAll();

        // eng: Returns the length of stream
        // rus: Возвращает размер потока
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern uint Magic_StreamGetLength(int hmStream);

        // eng: Returns current stream position
        // rus: Возвращает текущую позицию в потоке
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern uint Magic_StreamGetPosition(int hmStream);

        // eng: Sets the position of stream that was opened in read only mode
        // rus: Устанавливает позицию потока, открытого в режиме чтения
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_StreamSetPosition(int hmStream, uint position);

        // eng: Returns current stream mode
        // rus: Возвращает текущий режим потока
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern MAGIC_STREAM_ENUM Magic_StreamGetMode(int hmStream);

        // eng: Sets new stream mode
        // rus: Устанавливает новый режим потока
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_StreamSetMode(int hmStream, MAGIC_STREAM_ENUM mode);

        // eng: Returns file name of stream opened from file
        // rus: Возвращает имя файла для потока, открытого из файла
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern IntPtr Magic_StreamGetFileName(int hmStream);

        // eng: Reads specified number of bytes from stream
        // rus: Читает из потока указанное количество байт
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_StreamRead(int hmStream, byte[] data, uint count);

        // eng: Writes specified number of bytes into stream
        // rus: Сохраняет в поток указанное количество байт
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_StreamWrite(int hmStream, byte[] data, uint count);

        // eng: Copying particle space from stream into emitter
        // rus: Копирование пространства частиц в эмиттер из потока
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_LoadArrayFromStream(int hmEmitter, int hmStream);

        // eng: Copying particle space from emitter into stream
        // rus: Копирование пространства частиц из эмиттера а поток
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern int Magic_SaveArrayToStream(int hmEmitter, int hmStream);

        // eng: Sets emitter animation position to left position of visibility range
        // rus: Устанавливает позицию анимации эмиттера на левую позицию интервала видимости
#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport ("__Internal")]
#else
        [DllImport("magic3d")]
#endif
        public static extern int Magic_EmitterToInterval1_Stream(int hmEmitter, float speed_factor, int hmStream);
    };
}