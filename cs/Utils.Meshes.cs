using System;
using System.Collections.Generic;
using System.Linq;

using Utils.Collections;

#if UNITY
using UnityEngine;
#endif

namespace Utils 
{
	public static class Meshes 
	{
		#if UNITY
        public static Mesh Combine(MeshFilter[] filters) 
        {
            CombineInstance[] combiner = new CombineInstance[filters.Length];

            for (int i = 0; i < filters.Length; i++)
            {
                combiner[i].subMeshIndex = 0;
                combiner[i].mesh = filters[i].sharedMesh;
                combiner[i].transform = filters[i].transform.localToWorldMatrix;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiner);

            return mesh;
        }
        #endif

        public static Mesh Icosahedron() 
		{
			Mesh mesh = new Mesh();
			Vector3[] newVertices = new Vector3[12];
			float numA = 3f;
			float numB = 2f;
			int a = 0;
			int b = 4;
			int c = 8;
			
			newVertices[0] = new Vector3(numA / 2f, numB / 2f, 0f);
			newVertices[1] = new Vector3(numA / 2f, -numB / 2f, 0f);
			newVertices[2] = new Vector3(-numA / 2f, -numB / 2f, 0f);
			newVertices[3] = new Vector3(-numA / 2f, numB / 2f, 0f);

			newVertices[4] = new Vector3(numB / 2f, 0f, numA / 2f);
			newVertices[5] = new Vector3(numB / 2f, 0f, -numA / 2f);
			newVertices[6] = new Vector3(-numB / 2f, 0f, -numA / 2f);
			newVertices[7] = new Vector3(-numB / 2f, 0f, numA / 2f);

			newVertices[8] = new Vector3(0f, numA / 2f, numB / 2f);
			newVertices[9] = new Vector3(0f, -numA / 2f, numB / 2f);
			newVertices[10] = new Vector3(0f, -numA / 2f, -numB / 2f);
			newVertices[11] = new Vector3(0f, numA / 2f, -numB / 2f);

			int[] newTriangles = { a+2, a+3, b+2, a+2, b+2, c+2, b+1, c+2, b+2, c+3, b+2, a+3, c+3, b+1, b+2, c+3, a+0, b+1, c+3, c+0, a+0, c+0, b+0, a+0, c+0, b+3, b+0, c+3, a+3, c+0, c+0, a+3, b+3, a+3, a+2, b+3, b+3, a+2, c+1, b+0, b+3, c+1, a+0, b+0, a+1, b+0, c+1, a+1, b+1, a+0, a+1, b+1, a+1, c+2, c+2, a+1, c+1, c+2, c+1, a+2 };
			mesh.vertices = newVertices;
			mesh.triangles = newTriangles;
			return mesh;
		}

		public static Mesh IcoSphere(int resolution) 
		{
			Mesh mesh = Icosahedron();
			Vector3[] borderVertices = new Vector3[2];
			Vector3[] previousBorderVertices = new Vector3[2];
			List<Vector3> newVertices = new List<Vector3>();
			List<int> newTriangles = new List<int>();
			float section = 0f;
			float lineSection = 0f;

			int pointsInside = 0;
			int triangleIndex = 0;
			int line = 0;
			int t = 0;
			int v = 0;

			v = 0; while (v < mesh.vertices.Length) { newVertices.Add(mesh.vertices[v]); v++; }

			while (t < mesh.triangles.Length) 
			{
				borderVertices[0] = mesh.vertices[mesh.triangles[t]];
				borderVertices[1] = mesh.vertices[mesh.triangles[t + 1]];
				previousBorderVertices[0] = borderVertices[0];
				previousBorderVertices[1] = borderVertices[1];

				v = 0;
				lineSection = 1f / ((float)resolution + 1f);
				while (v < resolution) 
				{
					newVertices.Add(Math2.Lerp(borderVertices[0], borderVertices[1], lineSection));
					lineSection += 1f / ((float)resolution + 1f);
					v++;
				}
				
				line = 0;
				lineSection = 1f / ((float)resolution + 1f);
				while (line < resolution) 
				{
					newVertices.Add(Math2.Lerp(mesh.vertices[mesh.triangles[t]], mesh.vertices[mesh.triangles[t + 2]], lineSection));
					borderVertices[0] = newVertices[newVertices.Count - 1];
					newVertices.Add(Math2.Lerp(mesh.vertices[mesh.triangles[t + 1]], mesh.vertices[mesh.triangles[t + 2]], lineSection));
					borderVertices[1] = newVertices[newVertices.Count - 1];
					lineSection += 1f / ((float)resolution + 1f);

					v = 0;
					pointsInside = 0;
					section = 1f / (float)(resolution - line);
					while (v < resolution - 1 - line) 
					{
						newVertices.Add(Math2.Lerp(borderVertices[0], borderVertices[1], section));
						section += 1f / (float)(resolution - line);
						pointsInside++;
						v++;
					}

					triangleIndex = 0;
					while (triangleIndex < resolution - line) 
					{
						newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
						newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));
						newTriangles.Add((triangleIndex == 0) ? newVertices.LastIndexOf(previousBorderVertices[0]) : newVertices.Count - ((2 + pointsInside + (resolution - line)) - (triangleIndex - 1)));

						newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
						newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));
						newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));

						triangleIndex++;
					}
					
					triangleIndex--;
					newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
					newTriangles.Add(newVertices.LastIndexOf(previousBorderVertices[1]));
					newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));

					previousBorderVertices[0] = borderVertices[0];
					previousBorderVertices[1] = borderVertices[1];
					line++;
				}
				
				line--;
				triangleIndex--;
				newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));
				newTriangles.Add(mesh.triangles[t + 2]);
				newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));
				t += 3;
			}

			mesh.vertices = newVertices.ToArray();
			mesh.triangles = newTriangles.ToArray();
			return mesh;
		}
	}
}