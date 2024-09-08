namespace MHUtils.UI
{
    using MHUtils;
    using System;
    using UnityEngine;

    internal class MarkerData
    {
        public Vector3i cellPosition;
        private Vector3 position;
        public GameObject displayObject;
        public VerticalMarkerManager.MarkerType markerType;
        public object owner;
        public GameObject container;
        public int groupHash;
        public bool dirty;

        public Vector3 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (this.position != value)
                {
                    this.position = value;
                }
            }
        }
    }
}

