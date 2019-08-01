// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.UI.Controls;
using Xamarin.Forms;
using System.Collections.Generic;
using System;
using ArcGISRuntime.Samples.Managers;

namespace ArcGISRuntimeXamarin.Samples.CustomDictionaryStyle
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Custom dictionary style",
        "Symbology",
        "Use a custom dictionary style (.stylx) to symbolize features using a variety of attribute values.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("751138a2e0844e06853522d54103222a")]
    public partial class CustomDictionaryStyle : ContentPage
    {
        // The custom dictionary style for symbolizing restaurants.
        private DictionarySymbolStyle _restaurantStyle;

        private List<string> _styleItems = new List<string>() {"Food: ", "Rating: ", "Price: ", "Health Score: ", "Name: " };

    public CustomDictionaryStyle()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                // Open the custom style file.
                string stylxPath = GetStyleFilePath();
                _restaurantStyle = await DictionarySymbolStyle.CreateFromFileAsync(stylxPath);

                // Create a new map with a streets basemap.
                Map map = new Map(Basemap.CreateStreetsVector());

                // Create the restaurants layer and add it to the map.
                FeatureLayer restaurantLayer = new FeatureLayer(new Uri("https://services2.arcgis.com/ZQgQTuoyBrtmoGdP/arcgis/rest/services/Redlands_Restaurants/FeatureServer/0"));
                map.OperationalLayers.Add(restaurantLayer);

                // Get the fields from the restaurant feature table.
                var restaurantTable = restaurantLayer.FeatureTable;
                await restaurantTable.LoadAsync();
                IReadOnlyList<Field> datasetFields = restaurantLayer.FeatureTable.Fields;

                // Build a list of numeric and text field names.
                var symbolFields = new List<string> { " " };
                foreach (Field fld in datasetFields)
                {
                    if (fld.FieldType != FieldType.Blob &&
                        fld.FieldType != FieldType.Date &&
                        fld.FieldType != FieldType.Geometry &&
                        fld.FieldType != FieldType.GlobalID &&
                        fld.FieldType != FieldType.Guid &&
                        fld.FieldType != FieldType.OID &&
                        fld.FieldType != FieldType.Raster)
                    {
                        symbolFields.Add(fld.Name);
                    }
                }

                // Show the fields in the combo boxes.
                FoodPicker.ItemsSource = symbolFields;


                // Set the map's initial extent to that of the restaurants.
                map.InitialViewpoint = new Viewpoint(restaurantLayer.FullExtent);

                // Set the map to the map view.
                MyMapView.Map = map;
            }
            catch (Exception ex)
            {
                Console.WriteLine("**Exception: " + ex.ToString());
            }
        }

        private static string GetStyleFilePath()
        {
            return DataManager.GetDataFolder("751138a2e0844e06853522d54103222a", "Restaurant.stylx");
        }

        private void Edit_Style_Clicked(object sender, System.EventArgs e)
        {
            // Disable button UI
            MyMapView.IsVisible = false;
            ButtonGrid.IsVisible = false;
            // Enable Listview for styles
            StylePicker.ItemsSource = _styleItems;
            StylePickerUI.IsVisible = true;
            
        }

        private void StylePicker_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Disable styles listview
            StylePickerUI.IsVisible = false;
            

            switch (e.SelectedItemIndex)
            {
                case 0:
                    FoodPickerUI.IsVisible = true;
                    FoodPicker.SelectedItem = null;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
            

        }

        private void Picker_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            char first = (StylePicker.SelectedItem as string)[0];
            switch (first)
            {
                case 'F':
                    FoodPickerUI.IsVisible = false;
                    FoodLabel.Text = _styleItems[0] = "Food: " + FoodPicker.SelectedItem as string;
                    break;
            }
            StylePicker.SelectedItem = null;
            MyMapView.IsVisible = true;
            ButtonGrid.IsVisible = true;
        }
    }
}
