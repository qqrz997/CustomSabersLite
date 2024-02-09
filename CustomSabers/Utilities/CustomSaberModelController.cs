using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomSaberModelController : SaberModelController
    {
        private Color _saberColor;

        public void SetColour(Color color)
        {
            _saberColor = color;
        }
    }
}
