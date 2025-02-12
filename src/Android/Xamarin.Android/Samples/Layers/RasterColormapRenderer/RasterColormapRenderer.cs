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
using ArcGISRuntime.Samples.Managers;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ArcGISRuntimeXamarin.Samples.RasterColormapRenderer
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Raster colormap renderer",
        "Layers",
        "Apply a colormap renderer to a raster.",
        "Pan and zoom to explore the effect of the colormap applied to the raster.")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("95392f99970d4a71bd25951beb34a508")]
    public class RasterColormapRenderer : Activity
    {
        // Hold references to the UI controls.
        private MapView _myMapView;

        // A single band raster file.
        private readonly string _rasterPath = DataManager.GetDataFolder("95392f99970d4a71bd25951beb34a508", "shasta", "ShastaBW.tif");

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Raster colormap renderer";

            CreateLayout();
            Initialize();
        }

        private async void Initialize()
        {
            // Add an imagery basemap.
            Map map = new Map(Basemap.CreateImagery());

            // Load the raster file.
            Raster rasterFile = new Raster(_rasterPath);

            // Create the layer.
            RasterLayer rasterLayer = new RasterLayer(rasterFile);

            // Create a color map where values 0-149 are red and 150-249 are yellow.
            IEnumerable<Color> colors = new int[250]
               .Select((c, i) => i < 150 ? Color.Red : Color.Yellow);

            // Create a colormap renderer.
            ColormapRenderer colormapRenderer = new ColormapRenderer(colors);

            // Set the colormap renderer on the raster layer.
            rasterLayer.Renderer = colormapRenderer;

            // Add the layer to the map.
            map.OperationalLayers.Add(rasterLayer);

            // Add map to the mapview.
            _myMapView.Map = map;

            try
            {
                // Wait for the layer to load.
                await rasterLayer.LoadAsync();

                // Set the viewpoint.
                await _myMapView.SetViewpointGeometryAsync(rasterLayer.FullExtent, 15);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CreateErrorDialog(e.Message);
            }
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app.
            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

            // Add the map view to the layout.
            _myMapView = new MapView();
            layout.AddView(_myMapView);

            // Show the layout in the app.
            SetContentView(layout);
        }

        private void CreateErrorDialog(string message)
        {
            // Create a dialog to show message to user.
            AlertDialog alert = new AlertDialog.Builder(this).Create();
            alert.SetMessage(message);
            alert.Show();
        }
    }
}