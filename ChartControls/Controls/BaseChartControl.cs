using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ChartControls
{
    public abstract class BaseChartControl : Control
    {
        public BaseChartControl()
        {
            LoadStyle(this.GetType().FullName);
        }

        #region Base Style

        private static Dictionary<string, int> _loadedTypes;

        private static void LoadStyle(string typeName)
        {
            _loadedTypes = _loadedTypes ?? new Dictionary<string, int>();

            if (!_loadedTypes.ContainsKey(typeName))
            {
                _loadedTypes[typeName] = 1;
                var type = Type.GetType(typeName);
                DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            }
            else
                _loadedTypes[typeName]++;
        }

        #endregion
    }
}