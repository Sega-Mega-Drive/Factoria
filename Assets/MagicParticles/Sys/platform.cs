//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    public class Platform
    {
        // eng: Returns path to folder with emitters
        // rus: ���������� ���� � ����� � ptc-�������
        public static string ptc_path = "mp_ptc/";

        // eng: Returns path to folder with textures
        // rus: ���������� ���� � ����� � ����������
        public static string texture_path = "mp_textures/";

        // eng: Returns path to folder which could be used by wrapper to store temporary files
        // rus: ���������� ���� � ��������� �����
        public static string temp_path = Application.persistentDataPath;
    };
}