using CustomSabersLite.Utilities;
using SiraUtil.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using CustomSaber;

namespace CustomSabersLite.Components
{
    internal class CSLSaberModelController : SaberModelController, IColorable
    {
        private Color? color;

        private CSLSaber customSaberInstance;

        private CSLSaberTrail customTrailInstance;

        public Color Color
        {
            get => color.GetValueOrDefault();
            set => SetColor(value);
        }

        public void SetColor(Color color)
        {
            this.color = color;
            // set the color of the saber here!!!!
            
            if (customSaberInstance == null)
            {
                try
                {
                    customSaberInstance = transform.parent.GetComponentsInChildren<CSLSaber>().FirstOrDefault();
                }
                catch
                {
                    customSaberInstance = null;
                }
            }

            if (customTrailInstance == null)
            {
                try
                {
                    customTrailInstance = transform.parent.GetComponentsInChildren<CSLSaberTrail>().FirstOrDefault();
                }
                catch
                {
                    customTrailInstance = null;
                }
            }

            if (customSaberInstance != null)
            {
                customSaberInstance.SetColor(color);
            }
            if (customTrailInstance != null)
            {
                customTrailInstance.SetColor(color);
            }
        }
    }
}
