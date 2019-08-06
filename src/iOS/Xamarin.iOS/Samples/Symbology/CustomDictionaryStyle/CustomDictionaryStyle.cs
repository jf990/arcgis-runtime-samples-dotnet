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
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace ArcGISRuntimeXamarin.Samples.CustomDictionaryStyle
{
    [Register("CustomDictionaryStyle")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Custom dictionary style",
        "Symbology",
        "Use a custom dictionary style (.stylx) to symbolize features using a variety of attribute values.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("751138a2e0844e06853522d54103222a")]
    public class CustomDictionaryStyle : UIViewController
    {
        // Hold references to UI controls.
        private MapView _myMapView;
        private UIView _mainView;
        private UIView _pickerView;
        private UITableView table;
        private UIBarButtonItem _editButton;
        private UIBarButtonItem _backButton;
        private UITableView _tableView;

        // The custom dictionary style for symbolizing restaurants.
        private DictionarySymbolStyle _restaurantStyle;

        // User selections for style dictionary.
        private Dictionary<string, string> _styleItems = new Dictionary<string, string>();

        // Variables for style editing user interface.
        private bool _pickingCategory = true;

        private string _currentStyle;

        // List of all field options.
        private List<string> _symbolFields;

        public CustomDictionaryStyle()
        {
            Title = "Custom dictionary style";
        }

        private async void Initialize()
        {
            try
            {
                _styleItems.Add("Food", "Style");
                _styleItems.Add("Rating", "Rating");
                _styleItems.Add("Price", "Price");
                _styleItems.Add("Health Score", "Inspection");
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
                _symbolFields = new List<string> { " " };
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
                        _symbolFields.Add(fld.Name);
                    }
                }

                // Set the map's initial extent to that of the restaurants.
                map.InitialViewpoint = new Viewpoint(restaurantLayer.FullExtent);

                // Set the map to the map view.
                _myMapView.Map = map;
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

        private void Edit_Click(object sender, EventArgs e)
        {
            ChangeToPickerView();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            View = _mainView;
        }

        public void Go_To_Field(string style)
        {
            _tableView.Source = new FieldTableSource(_symbolFields, style, this);
            _tableView.ReloadData();
        }

        public void Set_Field(string style, string field)
        {
            _styleItems[style] = field;
            View = _mainView;
        }

        public override void LoadView()
        {
            // Create the view with the map.
            _mainView = new UIView();
            _mainView.BackgroundColor = UIColor.White;

            _myMapView = new MapView();
            _myMapView.TranslatesAutoresizingMaskIntoConstraints = false;

            _editButton = new UIBarButtonItem();
            _editButton.Title = "Edit style";
            _editButton.Clicked += Edit_Click;

            UIToolbar toolbar = new UIToolbar();
            toolbar.TranslatesAutoresizingMaskIntoConstraints = false;
            toolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                _editButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };

            // Add the views.
            _mainView.AddSubviews(_myMapView, toolbar);

            View = _mainView;

            // Lay out the views.
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                _myMapView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
                _myMapView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                _myMapView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                _myMapView.BottomAnchor.ConstraintEqualTo(toolbar.TopAnchor),

                toolbar.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),
                toolbar.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                toolbar.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
            });
        }

        private void ChangeToPickerView()
        {
            // Create the view for the pickers.
            _pickerView = new UIView();
            _pickerView.BackgroundColor = UIColor.White;

            _tableView = new UITableView(View.Bounds);
            _tableView.TranslatesAutoresizingMaskIntoConstraints = false;
            _tableView.Source = new StyleTableSource(_styleItems, this);

            _backButton = new UIBarButtonItem();
            _backButton.Title = "back";
            _backButton.Clicked += Back_Click;

            UIToolbar pickerToolbar = new UIToolbar();
            pickerToolbar.TranslatesAutoresizingMaskIntoConstraints = false;
            pickerToolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                _backButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };

            // Add the views.
            _pickerView.AddSubviews(_tableView, pickerToolbar);

            View = _pickerView;

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                _tableView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
                _tableView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                _tableView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                _tableView.BottomAnchor.ConstraintEqualTo(pickerToolbar.TopAnchor),

                pickerToolbar.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),
                pickerToolbar.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                pickerToolbar.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor)
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();
        }
    }
    class StyleTableSource : UITableViewSource
    {
        string CellIdentifier = "TableCell";
        Dictionary<string, string> styleItems;
        CustomDictionaryStyle _sample;

        public StyleTableSource(Dictionary<string, string> items, CustomDictionaryStyle sample)
        {
            styleItems = items;
            _sample = sample;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return styleItems.Keys.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            KeyValuePair<string, string> item = styleItems.ToList()[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            { cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier); }

            cell.TextLabel.Text = item.Key + " : " + item.Value;

            return cell;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _sample.Go_To_Field(styleItems.Keys.ToList()[indexPath.Row]);
        }
    }

    class FieldTableSource : UITableViewSource
    {
        string CellIdentifier = "TableCell";
        List<string> fields;
        CustomDictionaryStyle _sample;
        string _style;

        public FieldTableSource(List<string> items, string style, CustomDictionaryStyle sample)
        {
            fields = items;
            _sample = sample;
            _style = style;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return fields.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            { cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier); }

            cell.TextLabel.Text = fields[indexPath.Row];
            return cell;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _sample.Set_Field(_style, fields[indexPath.Row]);
        }
    }
}