using UnityEngine;

namespace Virsabi.Extensions {
    /// <summary>
    /// Extensions for the Vector3-class.
    /// @ Made by Mikkel S. K. Mogensen (mm@virsabi.com)
    /// </summary>
    public static class Vector3Extensions {
    
        /// <summary>
        /// Isolates the xy-components of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 xyIsolate(this Vector3 vec) {
            return new Vector3(vec.x, vec.y, 0);
        }

        /// <summary>
        /// Isolates the xz-components of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 xzIsolate(this Vector3 vec) {
            return new Vector3(vec.x, 0, vec.z);
        }

        /// <summary>
        /// Isolates the yz-components of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 yzIsolate(this Vector3 vec) {
            return new Vector3(0, vec.y, vec.z);
        }

        /// <summary>
        /// Isolates the x-component of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 xIsolate(this Vector3 vec) {
            return new Vector3(vec.x, 0, 0);
        }

        /// <summary>
        /// Isolates the y-components of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 yIsolate(this Vector3 vec) {
            return new Vector3(0, vec.y, 0);
        }

        /// <summary>
        /// Isolates the z-components of a Vector3 and returns it as a Vector3.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 zIsolate(this Vector3 vec) {
            return new Vector3(0, 0, vec.z);
        }

        /// <summary>
        /// Finds the intersection of an infinite ray with a plane in 3D and checks the dot product between the ray vector and plane normal.
        /// </summary>
        /// <param name="rayVector"></param>
        /// <param name="rayPoint"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <param name="isPointInPositiveDirection"></param>
        /// <returns></returns>
        public static Vector3 IntersectWithPlane(Vector3 rayVector, Vector3 rayPoint, Vector3 planeNormal, Vector3 planePoint, out bool isPointInPositiveDirection) {
            var diff = rayPoint - planePoint;
            var diffNormDot = Vector3.Dot(diff, planeNormal);
            var rayNormDot = Vector3.Dot(rayVector, planeNormal);
            var scalar = diffNormDot / rayNormDot;
                        
            isPointInPositiveDirection = rayNormDot < 0;
            
            return rayPoint - rayVector * scalar;
        }
       
        /// <summary>
        /// Finds the intersection of an infinite ray with a plane in 3D.
        /// </summary>
        /// <param name="rayVector"></param>
        /// <param name="rayPoint"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static Vector3 IntersectWithPlane(Vector3 rayVector, Vector3 rayPoint, Vector3 planeNormal, Vector3 planePoint) {
            var diff = rayPoint - planePoint;
            var diffNormDot = Vector3.Dot(diff, planeNormal);
            var rayNormDot = Vector3.Dot(rayVector, planeNormal);
            var scalar = diffNormDot / rayNormDot;
                                    
            return rayPoint - rayVector * scalar;
        }
    }
}