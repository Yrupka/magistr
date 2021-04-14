using System;
using System.Collections.Generic;

[Serializable]
public class Engine_options_lab_1
{
    [Serializable]
    public struct struct_rpms {
        public int rpm;
        public float moment;
        public float consumption;
    }

    public string engine_name;
    public string car_name;
    public string[] hints;
    public int fuel_amount;
    public int heat_time;
    public int interpolation;
    public float lever_length;
    public List<struct_rpms> rpms;

    public float max_moment;

    public List<int> Get_list_rpm()
    {
        List<int> list = new List<int>();
        foreach(struct_rpms rpm in rpms)
            list.Add(rpm.rpm);
        return list;
    }

    public List<float> Get_list_moment()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms rpm in rpms)
            list.Add(rpm.moment);
        return list;
    }

    public List<float> Get_list_consumption()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms rpm in rpms)
            list.Add(rpm.consumption);
        return list;
    }
}
