using System;
using System.Collections.Generic;

[Serializable]
public class Engine_options_lab_2
{
    [Serializable]
    public struct struct_rpms {
        public int rpm;
        public float moment;
        public float consumption;
        public float deg;
        public float load;
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
    public float max_load;


    public List<float> Get_list_rpm()
    {
        List<float> list = new List<float>();
        foreach(struct_rpms item in rpms)
            list.Add(item.rpm);
        return list;
    }

    public List<float> Get_list_moment()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms item in rpms)
            list.Add(item.moment);
        return list;
    }

    public List<float> Get_list_consumption()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms item in rpms)
            list.Add(item.consumption);
        return list;
    }

    public List<float> Get_list_degree()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms item in rpms)
            list.Add(item.deg);
        return list;
    }

    public List<float> Get_list_load()
    {
        List<float> list = new List<float>();
        foreach (struct_rpms item in rpms)
            list.Add(item.load);
        return list;
    }

    // считает и устанавливает максимальные значения для некоторых параметров
    public void Calculate()
    {
        max_moment = UnityEngine.Mathf.Max(Get_list_moment().ToArray());
        max_load = UnityEngine.Mathf.Max(Get_list_load().ToArray());
    }
}
