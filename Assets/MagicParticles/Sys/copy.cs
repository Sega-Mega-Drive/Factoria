//-----------------------------------------------------------------------------
// Copyright (c) Astralax. All rights reserved.
// Author: Trynkin Victor
// Version: 3.3
//-----------------------------------------------------------------------------

using UnityEngine;

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MagicParticles
{
    public class MP_Copy
    {
		private int reference;
		private int ram;

		private uint emitter_id;

		// rus: Returns of emitter ID, for which file was created
		// rus: Возвращает идентификатор эмиттера, для которого создан файл
		public uint EmitterID { get { return emitter_id; } }

		public MP_Copy(MP_Emitter emitter) 
		{ 
			emitter_id=MP_Core.Magic_GetEmitterID(emitter.ID);
			reference=0;			
            ram = 0;
			IncReference(emitter);
		}

		// eng: Cleaning up
		// rus: Очистка
		public void Destroy() 
		{ 
			if (ram!=0)
			{
				MP_Core.Magic_StreamClose(ram);
                ram = 0;
			}

			reference=0;
		}

		// eng: Increasing of reference count
		// rus: Увеличение числа ссылок на файл
		public void IncReference(MP_Emitter emitter) 
		{ 
			if (reference==0)
			{				
				// сохранение в ОЗУ
                ram = MP_Core.Magic_StreamOpenMemory(null, 0, MP_Core.MAGIC_STREAM_ENUM.MAGIC_STREAM_WRITE);

				LoadParticles(emitter);
				reference++;
			}
			else if (!Manager.Instance.CopyMode)
			{
                reference++;
			}
		}

		// eng: Decreasing of reference count
		// rus: Уменьшение числа ссылок на файл
		public void DecReference() 
		{
			if (!Manager.Instance.CopyMode)
			{
				reference--;
				if (reference==0)
					Manager.Instance.DeleteCopy(this);
			}
		}

		// eng: Loading of particles from file to emitter
		// rus: Загрузка частиц из файла в эмиттер
		public void LoadParticles(MP_Emitter emitter) 
		{
            if (ram != 0)
			{
				MP_Core.Magic_StreamSetPosition(ram, 0);
				MP_Core.Magic_EmitterToInterval1_Stream(emitter.ID, 1.0f, ram);
			}
			else
			{
				Debug.Assert(false, "[MP_Copy] LoadParticles from file - not work");
			}
		}
	}
}