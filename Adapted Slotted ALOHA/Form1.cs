﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Adapted_Slotted_ALOHA.Properties.Settings;
using NLog;

namespace Adapted_Slotted_ALOHA
{
    internal partial class Form1 : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Form1()
        {
            InitializeComponent();
            tableLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel3.BorderStyle = BorderStyle.FixedSingle;
            InitializeUI(Default.NumberOfStations, Default.NumberOfColums);
            CreateStations(Default.NumberOfStations);
        }

        List<Label> _stationsUI = new List<Label>();
        List<Label> UINewPackages = new List<Label>();
        List<List<Label>> UIPackages = new List<List<Label>>();
        List<Station> _stations = new List<Station>();
        Server _server = new Server();
        private int currentFrame;

        private void InitializeUI(int numberOfStations, int numberOfColums)
        {
            tableLayoutPanel1.RowCount = numberOfStations;
            for (var i = 0; i < numberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    BackColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _stationsUI.Add(label);
                tableLayoutPanel1.Controls.Add(_stationsUI[i]);

            }

            tableLayoutPanel2.RowCount = numberOfStations;
            for (var i = 0; i < numberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                UINewPackages.Add(label);
                tableLayoutPanel2.Controls.Add(UINewPackages[i]);

            }

            tableLayoutPanel3.RowCount = numberOfStations;
            tableLayoutPanel3.ColumnCount = numberOfColums;
            for (var i = 0; i < numberOfStations; i++)
            {
                var stationPackages = new List<Label>();
                for (var j = 0; j < numberOfColums; j++)
                {
                    var label = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Height = 30,
                        Width = Default.WidthOfColums,
                        Margin = new Padding(0, 0, 10, 3),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    stationPackages.Add(label);
                }
                UIPackages.Add(stationPackages);
            }
            for (var ii = 0; ii < numberOfStations; ii++)
            {
                for (var jj = 0; jj < numberOfColums; jj++)
                {
                    tableLayoutPanel3.Controls.Add(UIPackages[ii][jj]);
                }
            }
        }

        private void CreateStations(int numberOfStations)
        {
            for (var i = 0; i < numberOfStations; i++)
            {
                var station = new Station();
                _stations.Add(station);
            }
        }

        private void GeneratePackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                if (!_stations[i].IsPackageExist())
                {
                    _stations[i].GeneratePackage();
                    _stations[i].GenerateBacklogTime();
                }
            }
            RepaintBackloggedPackages();
        }

        private void SendPackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _server.Frames[i, currentFrame] = _stations[i].Package();
            RepaintPackages(currentFrame);
        }

        private void DecreaseBacklogTimers()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _stations[i].DecreaseBacklogTime();
        }

        private void CheckCollision()
        {
            if (!_server.IsCollision(currentFrame))
            {
                for (var i = 0; i < Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, currentFrame))
                        _stations[i].DestroyPackage();
            }
            else if (_server.IsCollision(currentFrame))
                for (var i = 0; i < Default.NumberOfStations; i++)
                {
                    if (_server.IsPackageSent(i, currentFrame))
                        _stations[i].GenerateBacklogTime();
                }
            RepaintBackloggedPackages();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            GeneratePackages();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SendPackages();
            CheckCollision();
            DecreaseBacklogTimers();
            GeneratePackages();
            currentFrame++;
        }

        private void RepaintPackages(int selectedFrame)
        {
            for (var i = 0; i < Default.NumberOfColums && selectedFrame >= 0; i++, selectedFrame--)
                for (var j = 0; j < Default.NumberOfStations; j++)
                {
                    UIPackages[j][i].BackColor = Color.Transparent;
                    if (_server.IsPackageSent(j, selectedFrame))
                    {
                        UIPackages[j][i].BackColor = _server.IsCollision(selectedFrame) ? Color.DarkRed : Color.LimeGreen;
                    }
                    UIPackages[j][i].Text = selectedFrame.ToString();
                }
        }

        private void RepaintBackloggedPackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                UINewPackages[i].BackColor = Color.Transparent;
                UINewPackages[i].Text = "";
                if (_stations[i].IsPackageExist())
                {
                    UINewPackages[i].Text = _stations[i].BacklogTime().ToString();
                }
            }
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                for (var j = 0; j < currentFrame; j++)
                {
                    _logger.Debug($"Frames[{i}][{j}]={_server.Frames[i, j]}");
                }
            }
        }
    }
}
