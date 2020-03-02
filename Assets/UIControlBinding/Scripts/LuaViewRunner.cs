using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace SDGame.UITools
{
    public class LuaViewRunner : MonoBehaviour, IBindableUI
    {
        public string viewClassName { get; set; }
        public LuaTable luaUI { get; private set; }

        public LuaTable BindLua(string viewClassName)
        {
            this.viewClassName = viewClassName;

            // TODO
            return null;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
