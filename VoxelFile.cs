using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class VoxelFile {

    public static byte[] Serialize(VoxelModel model){
        string data = "";
        data += model.Name + "\n";

        //Adds a comma separated list of dimensions: 2,2,2
        data += model.Size.x + "," + model.Size.y + "," + model.Size.z + "\n";

        //Adds a comma separated list of values: 0,0,2,3
        for (int i = 0; i < model.Data.Length; i++){
            data += i + (i < model.Data.Length - 1 ? "," : "");
        }

        return Encoding.ASCII.GetBytes(data);
    }

    public static VoxelModel Deserialize(byte[] data){
        string str = Encoding.ASCII.GetString (data);
        string[] lines = str.Split ('\n');
        if (lines.Length != 3)
            return null;

        string name = lines[0];
        string[] sizes = lines[1].Split (','), voxData = lines[2].Split (',');
        if (sizes.Length != 3)
            return null;

        int x = int.Parse(sizes[0]), y = int.Parse(sizes[1]), z = int.Parse (sizes[2]);
        if (voxData.Length != x * y * z)
            return null;

        int[] d = new int[x * y * z];
        for (int i = 0; i < d.Length; i++){
            d[i] = int.Parse (voxData[i]);
        }

        return new VoxelModel (x, y, z, name, d);
    }
}
