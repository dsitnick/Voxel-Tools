using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelModel {

    public const int AIR = -1;

    public readonly Vector3Int Size;
    public readonly string Name;

    //Returns a deep copy of the data for iteration purposes
    public int[] Data {
        get {
            int[] result = new int[data.Length];
            for (int i = 0; i < data.Length; i++) { result[i] = data[i]; }
            return result;
        }
    }

    private int[] data;

    //Creates an empty voxel model with the given size dimensions
    public VoxelModel (int x, int y, int z, string name) {
        Size = new Vector3Int (x, y, z);
        Name = name;
        data = new int[x * y * z];
        for (int i = 0; i < data.Length; i++)
            data[i] = AIR;
    }

    public VoxelModel(int x, int y, int z, string name, int[] d){
        Size = new Vector3Int (x, y, z);
        Name = name;
        data = new int[x * y * z];
        for (int i = 0; i < d.Length; i++){
            data[i] = d[i];
        }
    }

    
    //Places the value at the given coordinate, logs an error iff out of bounds
    public void Place (int x, int y, int z, int value) {
        if (!inBounds(x, y, z)){
            Debug.LogError ("Placed " + value + " out of bounds {" + x + "," + y + "," + z + "} Size {" + Size.ToString() + "}");
            return;
        }
        data[index (x, y, z)] = value;
    }

    
    //Returns the voxel value at the given coordinate, or AIR iff out of bounds
    public int Get(int x, int y, int z){
        if (!inBounds (x, y, z))
            return AIR;

        return data[index (x, y, z)];
    }

    #region Mesh

    
    //Builds a mesh from the voxel data, and given scale
    public Mesh BuildMesh (float scale) {
        List<Vector3> verts = new List<Vector3> (), norms = new List<Vector3> ();
        List<int> tris = new List<int> ();
        List<Color> colors = new List<Color> ();

        int vertCount;

        //For each coordinate
        for (int x = 0; x < Size.x; x++){
            for (int y = 0; y < Size.y; y++) {
                for (int z = 0; z < Size.z; z++) {
                    if (isBlock(x, y, z)){
                        //Space is occupied, build a voxel here

                        //Offset of this voxel from the center
                        Vector3 pos = new Vector3 (x, y, z) + (Vector3.one / 2f) - new Vector3(Size.x, Size.y, Size.z) / 2f;

                        //Color of this voxel
                        Color c = VoxelColors.Colors[Get (x, y, z)];

                        //For each side of this voxel
                        for (int side = 0; side < 6; side++){

                            //Avoid building the side if it's adjacent to another block
                            if (isBlock (x + nbrX[side], y + nbrY[side], z + nbrZ[side]))
                                continue;

                            //Triangle indices based on number of vertices + offsets
                            vertCount = verts.Count;
                            foreach (int t in triOffsets)
                                tris.Add (t + vertCount);

                            //For each vertex of this side
                            for (int i = 0; i < 4; i++) {
                                verts.Add (((sideRotations[side] * sidePositions[i]) + pos) * scale);
                                norms.Add (normals[side]);
                                colors.Add (c);
                            }


                        }
                    }
                }
            }
        }

        //Assemble mesh with built data
        Mesh mesh = new Mesh ();
        mesh.vertices = verts.ToArray ();
        mesh.normals = norms.ToArray ();
        mesh.triangles = tris.ToArray ();
        mesh.colors = colors.ToArray ();

        return mesh;
    }

    
    //Builds a mesh from the voxel data
    public Mesh BuildMesh () { return BuildMesh (1); }

    //4 positions of each side
    private static Vector3[] sidePositions = {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f)
    };

    //The X, Y, and Z offsets for each side, used to check if neighbor is occupied
    private static int[] nbrX = { -1, 1, 0, 0, 0, 0 };
    private static int[] nbrY = { 0, 0, -1, 1, 0, 0 };
    private static int[] nbrZ = { 0, 0, 0, 0, -1, 1 };

    //The index offsets for the two triangles for each side
    private static int[] triOffsets = {
        0, 1, 2,
        0, 2, 3
    };

    //Normal vector for each side
    private static Vector3[] normals = {
        Vector3.left, Vector3.right, Vector3.down, Vector3.up, Vector3.back, Vector3.forward
    };

    //Rotational data for each side, to put positions in place
    private static Quaternion[] sideRotations = {
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, -90, 0),
        Quaternion.Euler(-90, 0, 0),
        Quaternion.Euler(90, 0, 0),
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 180, 0)
    };

    #endregion

    #region Util

    
    //Returns the index equivalent to the given coordinate
    private int index (int x, int y, int z) {
        return x + (y * Size.x) + (z * Size.y * Size.x);
    }

    
    //Returns whether the given coordinate is nonnegative and within the size
    private bool inBounds(int x, int y, int z){
        return x >= 0 && y >= 0 && z >= 0 && x < Size.x && y < Size.y && z < Size.z;
    }

    
    //Returns whether the given coordinate is occupied by a block, allows for out of bounds
    private bool isBlock(int x, int y, int z){
        return Get (x, y, z) != AIR;
    }

    #endregion

}
