using System;
using System.Collections.Generic;

[Serializable]
public class Engine_options_lab_2
{
    [Serializable]
    public struct struct_rpms {
        public float rpm;
        public float moment;
        public float consumption;
        public float deg;

        public struct_rpms(float rpm, float moment, float consumption, float deg)
        {
            this.rpm = rpm;
            this.moment = moment;
            this.consumption = consumption;
            this.deg = deg;
        }
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

    public void Set_data(
        List<float> rpm, List<float> moment, List<float> consumption, List<float> deg)
    {
        rpms.Clear();
        for (int i = 0; i < rpm.Count; i++)
            rpms.Add(new struct_rpms(rpm[i], moment[i], consumption[i], deg[i]));
    }

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
}
