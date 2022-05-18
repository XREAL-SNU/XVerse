using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XEditor.CustomSearchWindow
{
    public class SearchObjectAttribute : PropertyAttribute
    {
        public Type searchObjectType;
        public SearchObjectAttribute(Type searchObjectType)
        {
            this.searchObjectType = searchObjectType;
        }
    }
}