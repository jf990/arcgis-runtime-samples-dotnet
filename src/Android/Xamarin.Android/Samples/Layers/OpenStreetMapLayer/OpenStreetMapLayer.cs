// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Android.App;
using Android.OS;
using Android.Widget;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;

namespace ArcGISRuntimeXamarin.Samples.OpenStreetMapLayer
{
    [Activity (ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "OpenStreetMap layer",
        "Layers",
        "Display an OpenStreetMap layer as a basemap.",
        "")]
    public class OpenStreetMapLayer : Activity
    {
        // Create and hold reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "OpenStreetMap layer";

            CreateLayout();
            Initialize();
        }

        private void Initialize()
        {
            // Create the OpenStreetMap basemap.
            Basemap osmBasemap = Basemap.CreateOpenStreetMap();

            // Create the map with the OpenStreetMap basemap.
            Map osmMap = new Map(osmBasemap);

            // Show the map in the view.
            _myMapView.Map = osmMap;
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app.
            var layout = new LinearLayout(this) {Orientation = Orientation.Vertical};

            // Add the map view to the layout.
            layout.AddView(_myMapView);

            // Show the layout in the app.
            SetContentView(layout);
        }
    }
}