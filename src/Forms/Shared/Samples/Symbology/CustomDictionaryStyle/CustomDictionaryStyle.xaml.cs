// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using ArcGISRuntime.Samples.Managers;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

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

        private Dictionary<string, string> _styleItems = new Dictionary<string, string>();

        private bool pickingCategory = true;
        private string _currentStyle;
        private List<string> symbolFields;

        public CustomDictionaryStyle()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                //< "Food", "" >, "Rating: ", "Price: ", "Health Score: ", "Name: "
                _styleItems.Add("Food", "");
                _styleItems.Add("Rating", "");
                _styleItems.Add("Price", "");
                _styleItems.Add("Health Score", "");
                _styleItems.Add("Name", "");

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
                symbolFields = new List<string> { " " };
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
                //FoodPicker.ItemsSource = symbolFields;

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
        private void Apply_Clicked(object sender, EventArgs e)
        {
            // Create overrides for expected field names that are different in this dataset.
            var styleToFieldMappingOverrides = new Dictionary<string, string>
            {
                { "style", _styleItems["Food"] },
                { "healthgrade", _styleItems["Health Score"]},
                { "rating", _styleItems["Rating"] },
                { "price", _styleItems["Price"] },
                { "opentimesun", "opensun" },
                { "closetimesun", "closesun" },
                { "opentimemon", "openmon" },
                { "closetimemon", "closemon" },
                { "opentimetue", "opentue" },
                { "closetimetue", "closetue" },
                { "opentimewed", "openwed" },
                { "closetimewed", "closewed" },
                { "opentimethu", "openthu" },
                { "closetimethu", "closethu" },
                { "opentimefri", "openfri" },
                { "closetimefri", "closefri" },
                { "opentimesat", "opensat" },
                { "closetimesat", "closesat" }
            };

            // Create overrides for expected text field names (if any).
            string labelField = _styleItems["Name"] != null ? _styleItems["Name"] : "";
            Dictionary<string, string> textFieldOverrides = new Dictionary<string, string>
                {
                    { "name", labelField }
                };

            // Set the text visibility configuration setting.
            _restaurantStyle.Configurations.ToList().Find(c => c.Name == "text").Value = NameSwitch.IsToggled ? "ON" : "OFF";

            // Create the dictionary renderer with the style file and the field overrides.
            DictionaryRenderer dictRenderer = new DictionaryRenderer(_restaurantStyle, styleToFieldMappingOverrides, textFieldOverrides);

            // Apply the dictionary renderer to the layer.
            var restaurantLayer = MyMapView.Map.OperationalLayers.First() as FeatureLayer;
            restaurantLayer.Renderer = dictRenderer;
        }

        private void ApplySimpleRendererClicked(object sender, EventArgs e)
        {
            // Apply a simple renderer that shows all points with the same marker symbol.
            SimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Black, 12);
            SimpleRenderer rendrr = new SimpleRenderer(markerSymbol);
            var restaurantLayer = MyMapView.Map.OperationalLayers.First() as FeatureLayer;
            restaurantLayer.Renderer = rendrr;
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
            PickerText.Text = "Select a style to edit.";
        }

        private void StylePicker_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Go to selected category
            if (pickingCategory)
            {
                _currentStyle = ((KeyValuePair<string, string>)e.SelectedItem).Key;
                StylePicker.ItemsSource = symbolFields;
                pickingCategory = false;
                PickerText.Text = "Select a field for " + _currentStyle;
            }
            else
            {
                // Set item based on category selection
                _styleItems[_currentStyle] = e.SelectedItem as string ?? "";
                pickingCategory = true;
                MyMapView.IsVisible = true;
                ButtonGrid.IsVisible = true;
                StylePickerUI.IsVisible = false;
            }
        }
    }
    internal class ListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FieldTemplate { get; set; }
        public DataTemplate StyleTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item.GetType() == typeof(KeyValuePair<string, string>))
            {
                return FieldTemplate;
            }
            if (item.GetType() == typeof(string))
            {
                return StyleTemplate;
            }
            return null;
        }
    }
}