using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace com.ihaiu
{
    public class CallbackStruct
    {
        public string                           filename;
        public Action<string, object, object[]> callback;
        public object[]                         args;

        public CallbackStruct(string filename, Action<string, object, object[]> callback, object[] args)
        {
            this.filename   = filename;
            this.callback   = callback;
            this.args       = args;
        }

        public void Call(object obj)
        {
            callback(filename, obj, args);
        }


        public void Clear()
        {
            filename = null;
            callback = null;
            args = null;
        }
            
    }
}