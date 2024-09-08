namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
    public class ProjectLocationToMap : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Location vector in degrees longitude and latitude. Typically returned by the Get Location Info action.")]
        public FsmVector3 GPSLocation;
        [HutongGames.PlayMaker.Tooltip("The projection used by the map.")]
        public MapProjection mapProjection;
        [ActionSection("Map Region"), HasFloatSlider(-180f, 180f)]
        public FsmFloat minLongitude;
        [HasFloatSlider(-180f, 180f)]
        public FsmFloat maxLongitude;
        [HasFloatSlider(-90f, 90f)]
        public FsmFloat minLatitude;
        [HasFloatSlider(-90f, 90f)]
        public FsmFloat maxLatitude;
        [ActionSection("Screen Region")]
        public FsmFloat minX;
        public FsmFloat minY;
        public FsmFloat width;
        public FsmFloat height;
        [ActionSection("Projection"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map.")]
        public FsmFloat projectedX;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map.")]
        public FsmFloat projectedY;
        [HutongGames.PlayMaker.Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
        public FsmBool normalized;
        public bool everyFrame;
        private float x;
        private float y;

        private void DoEquidistantCylindrical()
        {
            this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
            this.y = (this.y - this.minLatitude.Value) / (this.maxLatitude.Value - this.minLatitude.Value);
        }

        private void DoMercatorProjection()
        {
            this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
            float num = LatitudeToMercator(this.minLatitude.Value);
            float num2 = LatitudeToMercator(this.maxLatitude.Value);
            this.y = (LatitudeToMercator(this.GPSLocation.get_Value().y) - num) / (num2 - num);
        }

        private void DoProjectGPSLocation()
        {
            this.x = Mathf.Clamp(this.GPSLocation.get_Value().x, this.minLongitude.Value, this.maxLongitude.Value);
            this.y = Mathf.Clamp(this.GPSLocation.get_Value().y, this.minLatitude.Value, this.maxLatitude.Value);
            MapProjection mapProjection = this.mapProjection;
            if (mapProjection == MapProjection.EquidistantCylindrical)
            {
                this.DoEquidistantCylindrical();
            }
            else if (mapProjection == MapProjection.Mercator)
            {
                this.DoMercatorProjection();
            }
            this.x *= this.width.Value;
            this.y *= this.height.Value;
            this.projectedX.Value = this.normalized.Value ? (this.minX.Value + this.x) : (this.minX.Value + (this.x * Screen.width));
            this.projectedY.Value = this.normalized.Value ? (this.minY.Value + this.y) : (this.minY.Value + (this.y * Screen.height));
        }

        private static float LatitudeToMercator(float latitudeInDegrees)
        {
            return Mathf.Log(Mathf.Tan(((0.01745329f * Mathf.Clamp(latitudeInDegrees, -85f, 85f)) / 2f) + 0.7853982f));
        }

        public override void OnEnter()
        {
            if (this.GPSLocation.IsNone)
            {
                base.Finish();
            }
            else
            {
                this.DoProjectGPSLocation();
                if (!this.everyFrame)
                {
                    base.Finish();
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoProjectGPSLocation();
        }

        public override void Reset()
        {
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.GPSLocation = vector1;
            this.mapProjection = MapProjection.EquidistantCylindrical;
            this.minLongitude = -180f;
            this.maxLongitude = 180f;
            this.minLatitude = -90f;
            this.maxLatitude = 90f;
            this.minX = 0f;
            this.minY = 0f;
            this.width = 1f;
            this.height = 1f;
            this.normalized = true;
            this.projectedX = null;
            this.projectedY = null;
            this.everyFrame = false;
        }

        public enum MapProjection
        {
            EquidistantCylindrical,
            Mercator
        }
    }
}

