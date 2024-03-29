﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Extruder : MonoBehaviour {

	[Tooltip("The mesh's material")]
	public Material m_meshMaterial;

	[Tooltip("The shadow mode used by the mesh")]
	public ShadowCastingMode m_shadowMode;

	[Tooltip("How thick should the extrusion be (starting from extrusionHeight going both sides)")]
	[Range(0, 10)] public float m_extrusionDepth;

	[Tooltip("At what height should the extrusion start")]
	[Range(-10, 10)] public float m_extrusionHeight;

	[Tooltip("What offset should the extrusion have relative to the parent?")]
	public Vector2 m_offset;

	[Tooltip("The mesh's layer")]
	public string m_layer;

	public virtual void Start() {
		Extrude();
	}

	public abstract void Extrude();

	protected GameObject Create3DMeshObject(Vector2[] p_points, Transform p_parent, string p_name) {
		Triangulator triangulator = new Triangulator(p_points);
		int[] tris = triangulator.Triangulate();

		return Create3DMeshObject(p_points, tris, null, p_parent, p_name);
	}

	protected GameObject Create3DMeshObject(Vector2[] p_points, int[] p_tris, Vector2[] p_uvs, Transform p_parent, string p_name) {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[p_points.Length * 2];

        for(int i = 0; i < p_points.Length; i++) {
            vertices[i].x = p_points[i].x;
            vertices[i].y = p_points[i].y;
            vertices[i].z = m_extrusionHeight - m_extrusionDepth; // front vertex
            vertices[i + p_points.Length].x = p_points[i].x;
            vertices[i + p_points.Length].y = p_points[i].y;
            vertices[i + p_points.Length].z = m_extrusionHeight + m_extrusionDepth; // back vertex    
        }

        int[] triangles = new int[p_tris.Length * 2 + p_points.Length * 6];
        int count_tris = 0;

        for(int i = 0; i < p_tris.Length; i += 3) {
            // front vertices
            triangles[i] = p_tris[i];
            triangles[i + 1] = p_tris[i + 1];
            triangles[i + 2] = p_tris[i + 2];
        }

        count_tris += p_tris.Length;

        for(int i = 0; i < p_tris.Length; i += 3) {
            // back vertices
            triangles[count_tris + i] = p_tris[i + 2] + p_points.Length;
            triangles[count_tris + i + 1] = p_tris[i + 1] + p_points.Length;
            triangles[count_tris + i + 2] = p_tris[i] + p_points.Length;
        }

        count_tris += p_tris.Length;

        for(int i = 0; i < p_points.Length; i++) {
            // triangles around the perimeter of the object
            int n = (i + 1) % p_points.Length;

            triangles[count_tris] = i;
            triangles[count_tris + 1] = n;
            triangles[count_tris + 2] = i + p_points.Length;
            triangles[count_tris + 3] = n;
            triangles[count_tris + 4] = n + p_points.Length;
            triangles[count_tris + 5] = i + p_points.Length;

            count_tris += 6;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return CreateGameObjectFromMesh(mesh, p_parent, p_name);
	}

	protected GameObject CreateGameObjectFromMesh(Mesh p_mesh, Transform p_parent, string p_name) {
		GameObject meshObject = new GameObject(p_name);
		MeshRenderer renderer = (MeshRenderer) meshObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = meshObject.AddComponent(typeof(MeshFilter)) as MeshFilter;

		meshObject.transform.SetParent(p_parent);
		meshObject.layer = LayerMask.NameToLayer(m_layer);
		meshObject.transform.position += (Vector3)m_offset;
		renderer.material = m_meshMaterial;
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = m_shadowMode;
		filter.mesh = p_mesh;

		return meshObject;
	}
}
