using UnityEngine;

namespace GPCommon
{
    public static class GizmosUtil
    {

        public static void DrawBox(Vector2 position, Vector2 s)
        {
            var size = s / 2;

            var upperLeft = new Vector2(position.x -size.x, position.y + size.y);
            var upperRight = new Vector2(position.x +size.x, position.y + size.y);
            var lowerRight = new Vector2(position.x +size.x, position.y - size.y);
            var lowerLeft = new Vector2(position.x -size.x, position.y - size.y);

            Debug.DrawLine(upperLeft, upperRight);
            Debug.DrawLine(upperRight,lowerRight);
            Debug.DrawLine(lowerRight, lowerLeft);
            Debug.DrawLine(lowerLeft, upperLeft);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }


        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle);
        }

        private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color,
            float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
            if (gizmos)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
            }
            else
            {
                Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, up * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, down * arrowHeadLength, color);
            }
        }



    }
}
