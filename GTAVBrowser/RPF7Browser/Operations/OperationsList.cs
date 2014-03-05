/*
 
    Copyright (C) 2013, GTA-Network Team <contact at gta-network dot net>

    This software is provided 'as-is', without any express or implied
    warranty.  In no event will the authors be held liable for any damages
    arising from the use of this software.

    Permission is granted to anyone to use this software for any purpose,
    including commercial applications, and to alter it and redistribute it
    freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.
    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.
    3. This notice may not be removed or altered from any source distribution.
 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GTANETWORKV.Operations
{
    class OperationInfo<T>
    {
        public OperationInfo(string text, Action<T> operationFunction, Keys keyboardShortcut, bool isDefault, Func<T, bool> conditionFunction)
        {
            this._text = text;
            this._operation = operationFunction;
            this._keyboardShortcut = keyboardShortcut;
            this._isDefault = isDefault;
            this._checkCondition = conditionFunction;
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
        }

        private Action<T> _operation;
        public Action<T> Operation
        {
            get
            {
                return _operation;
            }
        }

        private Keys _keyboardShortcut;
        public Keys KeyboardShortcut
        {
            get
            {
                return _keyboardShortcut;
            }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
        }

        private Func<T, bool> _checkCondition;
        public Func<T, bool> CheckCondition
        {
            get
            {
                return _checkCondition;
            }
        }
    }

    class OperationsList<T> : List<OperationInfo<T>>
    {
        public void Add(string text, Action<T> operationFunction, Keys keyboardShortcut = Keys.None, bool isDefault = false, Func<T, bool> conditionFunction = null)
        {
            if (conditionFunction == null)
            {
                conditionFunction = delegate(T obj) { return true; };
            }
            Add(new OperationInfo<T>(text, operationFunction, keyboardShortcut, isDefault, conditionFunction));
        }
    }
}
