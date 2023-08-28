using UnityEngine;
namespace YGG
{
    public enum MeshResolution
    { Full = 0, Half, Quarter, Eighth, Sixteenth }
    public static class GameUtils
    {
        public static void SpawnObjectWithDestroyDuration(GameObject objectSpawn,Transform transform,bool isParent,float destroyDuration)
        {
            if(objectSpawn == null)return;
            Object.Destroy(Object.Instantiate(objectSpawn,transform.localPosition,Quaternion.identity,isParent ? transform : null),destroyDuration);
        }
        public static void SpawnObjectPositionWithDestroyDuration(GameObject objectSpawn,Vector3 position, float destroyDuration)
        {
            if (objectSpawn == null) return;
            Object.Destroy(Object.Instantiate(objectSpawn,position,Quaternion.identity),destroyDuration);
        }
        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        public static Mesh ConvertTerrainToMesh(TerrainData terrainData,MeshResolution meshResolution,Vector3 position)
        {
            int width, depth;
            Vector3[] vertices;
            int[] triangles;
            float[,] heightmapData;
            Vector3 meshScale;

            meshScale = terrainData.size;
            width = depth = terrainData.heightmapResolution;

            var res = (int)Mathf.Pow(2, (int)meshResolution);
            meshScale = new Vector3(meshScale.x / (width - 1) * res, meshScale.y, meshScale.z / (depth - 1) * res);
            heightmapData = terrainData.GetHeights(0, 0, width, depth);

            width = width / res;
            depth = depth / res;
            vertices = new Vector3[width* depth];
            triangles = new int[(width - 1) * (depth - 1) * 6];


            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertices[z * width + x] = Vector3.Scale(meshScale, new Vector3(z, heightmapData[x * res, z * res], x)) + position;
                }
            }
            int index = 0;
            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < depth - 1; z++)
                {
                    //012 213
                    triangles[index++] = (x * depth) + z;
                    triangles[index++] = (x * depth) + z + 1;
                    triangles[index++] = ((x + 1) * depth) + z;

                    triangles[index++] = ((x + 1) * depth) + z;
                    triangles[index++] = (x * depth) + z + 1;
                    triangles[index++] = ((x + 1) * depth) + z + 1;
                }
            }
            var mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;

        }
    }
}
