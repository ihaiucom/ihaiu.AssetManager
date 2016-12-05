using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public partial class AssetManager 
    {
        public int moduleId = 0;
        public Dictionary<int, LoadedModule> modules = new Dictionary<int, LoadedModule>();

        private LoadedModule _currentModule;
        public LoadedModule currentModule
        {
            get
            {
                if (_currentModule == null)
                {
                    if (modules.ContainsKey(moduleId))
                    {
                        _currentModule = modules[moduleId];
                    }
                    else
                    {
                        _currentModule = new LoadedModule(moduleId, this);
                        modules.Add(moduleId, _currentModule);
                    }
                }

                return _currentModule;
            }
        }

        public void OnOpenModule(int moduleId)
        {
            this.moduleId = moduleId;

            if (modules.ContainsKey(moduleId))
            {
                _currentModule = modules[moduleId];
            }
            else
            {
                _currentModule = new LoadedModule(moduleId, this);
                modules.Add(moduleId, _currentModule);
            }
            _currentModule.openCount++;
            _currentModule.referencedCount++;
        }

        public void OnCloseModule(int moduleId)
        {
            if (modules.ContainsKey(moduleId))
            {
                modules[moduleId].referencedCount--;
                modules[moduleId].Unload();
            }

            if (this.moduleId == moduleId)
            {
                OnOpenModule(0);
            }
        }


        void AddToModule(AssetInfo assetInfo)
        {
            currentModule.Add(assetInfo);
        }

    }
}