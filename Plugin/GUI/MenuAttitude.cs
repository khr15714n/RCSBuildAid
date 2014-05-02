/* Copyright © 2013-2014, Elián Hanisch <lambdae2@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using UnityEngine;

namespace RCSBuildAid
{
    public static class MenuAttitude
    {
        public static void onModeChange (PluginMode mode)
        {
            MainWindow.onDrawModeContent -= DrawContent;
            if (mode == PluginMode.Attitude) {
                MainWindow.onDrawModeContent += DrawContent;
            }
        }

        static void DrawContent ()
        {
            MarkerForces comv = RCSBuildAid.VesselForces;
            GUILayout.BeginHorizontal ();
            {
                if (hasAttitudeControl ()) {
                    GUILayout.BeginVertical (); 
                    {
                        GUILayout.Label ("Reference");
                        GUILayout.Label ("Direction");
                        GUILayout.Label ("Torque");
                        GUILayout.Label ("Thrust");
                    }
                    GUILayout.EndVertical ();
                    GUILayout.BeginVertical ();
                    {
                        MainWindow.referenceButton ();
                        MainWindow.directionButton ();
                        GUILayout.Label (comv.Torque().magnitude.ToString("0.## kNm"));
                        GUILayout.Label (comv.Thrust().magnitude.ToString("0.## kN"));
                    }
                    GUILayout.EndVertical ();
                } else {
                    GUILayout.Label ("No attitude control elements attached", MainWindow.style.centerText);
                }
            }
            GUILayout.EndHorizontal ();
        }

        static bool hasAttitudeControl ()
        {
            return RCSBuildAid.RCSlist.Count != 0;
        }
    }
}
