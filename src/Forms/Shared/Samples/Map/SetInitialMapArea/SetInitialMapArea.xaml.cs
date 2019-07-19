// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Xamarin.Forms;

namespace ArcGISRuntime.Samples.SetInitialMapArea
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Set initial map area",
        "Map",
        "This sample demonstrates how to set the initial viewpoint from envelope defined by minimum (x,y) and maximum (x,y) values. The map's InitialViewpoint is set to this viewpoint before the map is loaded into the MapView. Upon loading the map zoom to this initial area.",
        "")]
    public partial class SetInitialMapArea : ContentPage
    {
        public SetInitialMapArea()
        {
            InitializeComponent ();

            // Create the UI, setup the control references and execute initialization 
            Initialize();
        }

        private void Initialize()
        {
            // Create new Map with basemap
            Map myMap = new Map(Basemap.CreateImagery());

            // Create and set initial map area
            Envelope initialLocation = new Envelope(
                -12211308.778729, 4645116.003309, -12208257.879667, 4650542.535773,
                SpatialReferences.WebMercator);
            myMap.InitialViewpoint = new Viewpoint(initialLocation);

            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }
    }
}