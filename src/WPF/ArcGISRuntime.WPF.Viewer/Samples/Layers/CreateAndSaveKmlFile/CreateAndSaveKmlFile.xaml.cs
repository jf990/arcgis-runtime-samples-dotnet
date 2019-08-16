﻿// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Geometry = Esri.ArcGISRuntime.Geometry.Geometry;

namespace ArcGISRuntime.WPF.Samples.CreateAndSaveKmlFile
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Create and save KML file",
        "Layers",
        "Construct a KML document and save it as a KMZ file.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData()]
    public partial class CreateAndSaveKmlFile
    {
        private KmlDocument _kmlDocument;
        private KmlDataset _kmlDataset;
        private KmlLayer _kmlLayer;
        private KmlPlacemark _currentPlacemark;

        public CreateAndSaveKmlFile()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Create the map.
            MyMapView.Map = new Map(Basemap.CreateImagery());

            // Set the colors for the color picker.
            var propertylist = typeof(Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            List<Color> colorList = propertylist.Select(x => (Color)x.GetValue(x)).ToList();
            colorList.RemoveAt(0);

            ColorPicker.ItemsSource = colorList;
            ColorPicker.SelectedIndex = 0;

            // Set the images for the point icon picker.
            List<string> iconLinks = new List<string>()
            {
                "http://static.arcgis.com/images/Symbols/Shapes/BlueCircleLargeB.png",
                "http://static.arcgis.com/images/Symbols/Shapes/BlueDiamondLargeB.png",
                "http://static.arcgis.com/images/Symbols/Shapes/BluePin1LargeB.png",
                "http://static.arcgis.com/images/Symbols/Shapes/BluePin2LargeB.png",
                "http://static.arcgis.com/images/Symbols/Shapes/BlueSquareLargeB.png",
                "http://static.arcgis.com/images/Symbols/Shapes/BlueStarLargeB.png"
            };
            List<Uri> iconList = iconLinks.Select(x => new Uri(x)).ToList();

            IconPicker.ItemsSource = iconList;
            IconPicker.SelectedIndex = 0;

            // Set up a new kml document and kml layer.
            ResetKml();
        }

        private void ResetKml()
        {
            // Clear any existing layers from the map.
            MyMapView.Map.OperationalLayers.Clear();

            // Reset the most recently placed placemark.
            _currentPlacemark = null;

            // Create a new KmlDocument.
            _kmlDocument = new KmlDocument() { Name = "KML Sample Document" };

            // Create a Kml dataset using the Kml document.
            _kmlDataset = new KmlDataset(_kmlDocument);

            // Create the kmlLayer using the kmlDataset.
            _kmlLayer = new KmlLayer(_kmlDataset);

            // Add the Kml layer to the map.
            MyMapView.Map.OperationalLayers.Add(_kmlLayer);
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hide the base UI and enable the complete button.
                ShapesPanel.Visibility = Visibility.Hidden;
                CompleteButton.Visibility = Visibility.Visible;
                SaveResetGrid.IsEnabled = false;

                // Create variables for the sketch creation mode and color.
                SketchCreationMode creationMode;

                // Set the creation mode and UI based on which button called this method.
                switch (((Button)sender).Name)
                {
                    case nameof(PointButton):
                        creationMode = SketchCreationMode.Point;
                        InstructionsText.Text = "Tap to add a point.";
                        StyleText.Text = "Select an icon for the placemark.";
                        break;

                    case nameof(PolylineButton):
                        creationMode = SketchCreationMode.Polyline;
                        InstructionsText.Text = "Tap to add a vertex.";
                        StyleText.Text = "Select a color for the placemark.";
                        break;

                    case nameof(PolygonButton):
                        creationMode = SketchCreationMode.Polygon;
                        InstructionsText.Text = "Tap to add a vertex.";
                        StyleText.Text = "Select a color for the placemark.";
                        break;

                    default:
                        return;
                }

                // Get the user-drawn geometry.
                Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);

                // Project the geometry to WGS84 (WGS84 is required by the KML standard).
                Geometry projectedGeometry = GeometryEngine.Project(geometry, SpatialReferences.Wgs84);

                // Create a KmlGeometry using the new geometry.
                KmlGeometry kmlGeometry = new KmlGeometry(projectedGeometry, KmlAltitudeMode.ClampToGround);

                // Create a new placemark.
                _currentPlacemark = new KmlPlacemark(kmlGeometry);

                // Add the placemark to the KmlDocument.
                _kmlDocument.ChildNodes.Add(_currentPlacemark);

                // Enable the style editing UI.
                StyleBorder.Visibility = Visibility.Visible;
                MainUI.IsEnabled = false;

                // Choose whether to enable the icon picker or color picker.
                IconPicker.Visibility = creationMode == SketchCreationMode.Point ? Visibility.Visible : Visibility.Collapsed;
                ColorPicker.Visibility = creationMode != SketchCreationMode.Point ? Visibility.Visible : Visibility.Collapsed;
            }
            finally
            {
                // Reset the UI.
                ShapesPanel.Visibility = Visibility.Visible;
                CompleteButton.Visibility = Visibility.Collapsed;
                InstructionsText.Text = "Select the type of feature you would like to add.";

                // Enable the save and reset buttons.
                SaveResetGrid.IsEnabled = true;
            }
        }

        private void Apply_Style_Click(object sender, RoutedEventArgs e)
        {
            // Create a new style for the placemark.
            _currentPlacemark.Style = new KmlStyle();

            // Set the style for that Placemark.
            switch (_currentPlacemark.Geometries.FirstOrDefault().Type)
            {
                // Create a KmlIconStyle using the selected icon.
                case KmlGeometryType.Point:
                    _currentPlacemark.Style.IconStyle = new KmlIconStyle(new KmlIcon((Uri)IconPicker.SelectedItem), 1.0);
                    break;

                // Create a KmlLineStyle using the selected color value.
                case KmlGeometryType.Polyline:
                    _currentPlacemark.Style.LineStyle = new KmlLineStyle((Color)ColorPicker.SelectedItem, 8);
                    break;

                // Create a KmlPolygonStyle using the selected color value.
                case KmlGeometryType.Polygon:
                    _currentPlacemark.Style.PolygonStyle = new KmlPolygonStyle((Color)ColorPicker.SelectedItem);
                    _currentPlacemark.Style.PolygonStyle.IsFilled = true;
                    _currentPlacemark.Style.PolygonStyle.IsOutlined = false;
                    break;
            }

            // Reset the UI.
            StyleBorder.Visibility = Visibility.Collapsed;
            MainUI.IsEnabled = true;
        }

        private void No_Style_Click(object sender, RoutedEventArgs e)
        {
            // Reset the UI.
            StyleBorder.Visibility = Visibility.Collapsed;
            MainUI.IsEnabled = true;
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Finish the sketch.
                MyMapView.SketchEditor.CompleteCommand.Execute(null);
            }
            catch (ArgumentException)
            {
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Open a save dialog for the user.
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "kmz files (*.kmz)|*.kmz";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                // Write the KML document to the chosen path.
                await _kmlDocument.SaveAsAsync(saveFileDialog.FileName);
                MessageBox.Show("Item saved.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetKml();
        }
    }
}