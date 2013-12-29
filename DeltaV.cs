/* Copyright © 2013, Elián Hanisch <lambdae2@gmail.com>
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
    public class DeltaV : MonoBehaviour
    {
        public static float dV = 0f;
        public static float burnTime = 0f;

        CoMVectors CoM;

        float isp;
        float G = 9.81f;

        void Update ()
        {
            if (CoM == null) {
                CoM = RCSBuildAid.CoM.GetComponent<CoMVectors> ();
                if (CoM == null) {
                    return;
                }
            }

            float resource = 0;
            switch (RCSBuildAid.mode) {
            case DisplayMode.RCS:
                DCoM_Marker.Resource.TryGetValue ("MonoPropellant", out resource);
                break;
            case DisplayMode.Engine:
            default:
                dV = 0;
                burnTime = 0;
                return;
            }
            calcIsp ();
            float fullMass = CoM_Marker.Mass;
            float dryMass = fullMass - resource;
            dV = G * isp * Mathf.Log (fullMass / dryMass);

            float thrust = CoM.thrust.magnitude;
            burnTime = thrust < 0.001 ? 0 : resource * G * isp / thrust;
        }

        void calcIsp ()
        {
            float denominator = 0, numerator = 0;
            switch (RCSBuildAid.mode) {
            case DisplayMode.RCS:
                calcRCSIsp (ref numerator, ref denominator);
                break;
            case DisplayMode.Engine:
            default:
                isp = 0;
                return;
            }
            if (denominator == 0) {
                isp = 0;
                return;
            }
           isp = numerator / denominator; /* weighted mean */
        }

        void calcRCSIsp (ref float num, ref float den)
        {
            foreach (PartModule pm in RCSBuildAid.RCSlist) {
                ModuleForces forces = pm.GetComponent<ModuleForces> ();
                if (forces && forces.enabled) {
                    ModuleRCS mod = (ModuleRCS)pm;
                    float isp = mod.atmosphereCurve.Evaluate (0f);
                    foreach (VectorGraphic vector in forces.vectors) {
                        Vector3 thrust = vector.value;
                        isp = Vector3.Dot (isp * thrust.normalized, CoM.thrust.normalized);
                        /* calculating weigthed mean, RCS thrust magnitude is already "weigthed" */
                        num += thrust.magnitude * isp;
                        den += thrust.magnitude;
                    }
                }
            }
        }
    }
}
