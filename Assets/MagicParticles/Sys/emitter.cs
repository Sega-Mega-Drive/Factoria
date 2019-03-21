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
    public class MP_Emitter
    {
        // eng: emitter is not working 
        // rus: ������� �� ��������
        public const int MAGIC_STATE_STOP       = 0;

        // eng: emitter is updated and rendered 
        // rus: ������� ����������� � ��������
        public const int MAGIC_STATE_UPDATE		= 1;

        // eng: emitter interrupts, i.e. is working while there are "alive" particles 
        // rus: ������� ����������� � �������� �� ������� ����������� ���� ��������� ������, ����� ������� ������ �� ���������
        public const int MAGIC_STATE_INTERRUPT  = 2;

        // eng: Emitter is only rendered 
        // rus: ������� ������ ��������
        public const int MAGIC_STATE_VISIBLE    = 3;

        // eng: state
        // rus: ������
        private int state;
        // eng: the state of the emitter
        // rus: ������� ��������
        public int State { 
            get { return state; }

            set
            {
                if (this.state == value)
                    return;

                if (value == MAGIC_STATE_UPDATE && MP_Core.Magic_IsInterrupt(id))
                {
                    // eng: it is necessary to turn off interrupting of emitter work
                    // rus: ���������� ��������� ���������� ������ ��������
                    MP_Core.Magic_SetInterrupt(id, false);
                }

                if (value == MAGIC_STATE_STOP && state != MAGIC_STATE_INTERRUPT)
                {
                    // eng: unload particles from memory
                    // rus: ��������� ������������ ������ �� ������
                    MP_Core.Magic_Stop(id);
                }
                else if (value == MAGIC_STATE_UPDATE || value == MAGIC_STATE_INTERRUPT)
                {
                    // eng: start emitter
                    // rus: �������� �������
                    if (!first_restart)
                    {
                        // rus: ������� �������� �������� �� ������ � �������� ���������, ���������� ����������� ��������� �� ������
                        if (state == MAGIC_STATE_STOP || !InInterval)
                        {                            
                            if (copy != null)
                                copy.LoadParticles(this);
                        }
                    }

                    if (value == MAGIC_STATE_INTERRUPT)
                        MP_Core.Magic_SetInterrupt(id, true);
                }

                state = value;
            }
        }

        // eng: descriptor of emitter
        // rus: ���������� ��������
        private int id;
        // eng: Returning the descriptor of the emitter to work with API
        // ����������� ����������� ��������, ����� �������� � API
        public int ID { get { return id; } }

        public MagicEmitter Owner;

        // eng: coordinate z of emitter
        // rus: ���������� z ��������
        private float z;

        // eng: indicates that emitter does not work
        // rus: ������� ����, ��� ������� ��� �� ����������� �� ��������� ������� ��������
        private bool first_restart;

        // rus: file with particles copy
        // rus: ���� � ������ ������
        public MP_Copy copy;

        // eng: indicates that atlas for this emitter was created
        // rus: ������� ����, ��� ����� ��� ������� �������� ��� ��� ��������
        public bool is_atlas;

        // eng: the name of the emitter
        // rus: ��� ��������
        private string name;
        public string Name
        {
            get { return name; }            
        }        

        public bool InInterval
        {
            get { return MP_Core.Magic_InInterval(id); }
        }

        // ���� �� �������� ��� ���������� �������
        public string file;

        // ���� � ������� ������ �����
        public string path;

        public enum EnumNamingMode
        {
            Name,
            Ptc_Name,
            Ptc_Path_Name
        };

        public string GetFullName(EnumNamingMode mode)
        {
            switch (mode)
            {
                case EnumNamingMode.Ptc_Name: return System.IO.Path.GetFileNameWithoutExtension(file) + "/" + name; 
                case EnumNamingMode.Ptc_Path_Name: 
                    if (String.IsNullOrEmpty(path))
                        return System.IO.Path.GetFileNameWithoutExtension(file) + "/" + name; 
                    else
                        return System.IO.Path.GetFileNameWithoutExtension(file) + "/" + path + "/" + name;
            }

            return name;
        }

        public unsafe MP_Emitter(int emitter)
        {
            this.id = emitter;

            name = Magic.GetEmitterName(emitter); 

            z = 0.0f;
            first_restart = true;
            copy = null;            
            state = MAGIC_STATE_UPDATE;
            is_atlas = false;
        }

        public MP_Emitter(MP_Emitter origin)
        {
            state = origin.state;
            z = origin.z;
            first_restart = origin.first_restart;            

            is_atlas = origin.is_atlas;            

            name = origin.name;
            file = origin.file;

            if (origin.copy != null)
            {
                copy = origin.copy;
                copy.IncReference(this);
            }

            id = MP_Core.Magic_DuplicateEmitter(origin.id);
        }

        public void Destroy()
        {
            if (Owner)
            {
                // ������� ����� ���������
                Owner.Emitter = null;
            }
            else
            {
                if (copy != null)
                {
                    copy.DecReference();
                    copy = null;
                }

                MP_Core.Magic_UnloadEmitter(id);

                // ������� �� ���������
                Manager.Instance.DeleteEmitter(id);
            }
        }

        // eng: Restarting of emitter
        // rus: ��������� �������� �� ��������� �������
        public void Restart()
        {
            if (MP_Core.Magic_IsInterval1(id))
            {
                // eng: animation starts not from beginning
                // rus: �������� ���������� �� � ������
                if (copy == null)
                {
                    copy = Manager.Instance.FindCopy(MP_Core.Magic_GetEmitterID(id));
                    if (copy == null)
                        copy = Manager.Instance.AddCopy(this);
                }

                copy.LoadParticles(this);
            }
            else
            {
                MP_Core.Magic_Restart(id);
            }

            first_restart = false;
        }

        // eng: Updating emitter
        // rus: ���������� ��������
        public bool Update(double time)
        {
            if (state == MAGIC_STATE_UPDATE || state == MAGIC_STATE_INTERRUPT)
            {
                if (first_restart)
                    Restart();

                // rus: without interpolation a fixing step is only possible
                // rus: ��� ������������ �������� ������ ������������� ���
                if (!MP_Core.Magic_IsInterpolationMode(id))
                    time = MP_Core.Magic_GetUpdateTime(id);

                // eng: working of emitter is over
                // rus: ���������� �������� ���������
                if (!MP_Core.Magic_Update(id, time))
                {
                    State = MAGIC_STATE_STOP;
                    return true;
                }
            }

            return false;
        }

    }
}