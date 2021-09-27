using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Calculation_formulas
{
    private struct quartet
    {
        public float item_1;
        public float item_2;
        public float item_3;
        public float item_4;

        public quartet(float item_1, float item_2, float item_3, float item_4)
        {
            this.item_1 = item_1;
            this.item_2 = item_2;
            this.item_3 = item_3;
            this.item_4 = item_4;
        }
    }
    // функция вычисляющая интерполирующую состовляющую графика, label_x,y - значения исходной функции
    private static float Interpolate(float x, List<float> label_x, List<float> label_y)
    {
        float answ = 0f;
        for (int j = 0; j < label_x.Count; j++)
        {
            float l_j = 1f;
            for (int i = 0; i < label_x.Count; i++)
            {
                if (i == j)
                    l_j *= 1f;
                else
                    l_j *= (x - label_x[i]) / (label_x[j] - label_x[i]);
            }
            answ += l_j * label_y[j];
        }
        return answ;
    }

    // возвращает новые координаты точек по x с учетом интерполированных значений
    public static List<float> Interpolated_x(List<float> x, int interpolation)
    {
        List<float> interpolated_x = new List<float>();

        float dot_place_procent = 1f / (interpolation + 1);

        for (int j = 0; j < x.Count - 1; j++)
            if (x[j] != x[j + 1])
                for (int i = 0; i <= interpolation; i++)
                    interpolated_x.Add(Mathf.Lerp(x[j], x[j + 1], i * dot_place_procent));
            else
                interpolated_x.Add(x[j]);

        interpolated_x.Add(x[x.Count - 1]);

        return interpolated_x;
    }

    // возвращает координаты точек по y для переданных значений х
    public static List<float> Interpolated_y(List<float> x, List<float> y, List<float> x_for_calculate)
    {
        List<float> interpolated_y = new List<float>();
        if (x.Count == 1)
        {
            interpolated_y.Add(y[0]);
            goto x;
        }
            
        for (int i = 0; i < x_for_calculate.Count - 1; i++)
        {
            if (x_for_calculate[i] == x_for_calculate[i + 1])
                interpolated_y.Add(y[i]);
            else
                interpolated_y.Add(Interpolate(x_for_calculate[i], x, y));
        }
        int count = x_for_calculate.Count - 1;
        if (x_for_calculate[count] == x_for_calculate[count - 1])
            interpolated_y.Add(y[count]);
        else
            interpolated_y.Add(Interpolate(x_for_calculate[count], x, y));

        x:
        return interpolated_y;
    }

    public static (List<float>, List<float>, List<float>, List<float>) interpolate_3d(
        List<float> x, List<float> y, List<float> w, List<float> z, int interpolation)
    {
        List<quartet> rpms = new List<quartet>();
        for (int i = 0; i < x.Count; i++)
            rpms.Add(new quartet(x[i], y[i], w[i], z[i]));

        var groups = rpms.GroupBy(a => a.item_1);
        List<List<float>> arr_1 = new List<List<float>>();
        List<List<float>> arr_2 = new List<List<float>>();
        List<List<float>> arr_3 = new List<List<float>>();
        List<float> arr_4 = new List<float>();
        foreach (var item in groups)
        {
            List<float> value_1 = new List<float>();
            List<float> value_2 = new List<float>();
            List<float> value_3 = new List<float>();
            foreach (var it in item)
            {
                value_1.Add(it.item_2);
                value_2.Add(it.item_3);
                value_3.Add(it.item_4);
            }
            arr_1.Add(value_1);
            arr_2.Add(value_2);
            arr_3.Add(value_3);
            arr_4.Add(item.ElementAt(0).item_1);
        }

        // нужно посчитать сколько точек в каждой отдельной группе оборотов
        List<float> interpolated_z = new List<float>();
        List<float> interpolated_y = new List<float>();
        List<float> interpolated_w = new List<float>();
        List<float> interpolated_x = new List<float>();
        for (int i = 0; i < arr_1.Count; i++)
        {
            List<float> calculated_z = Interpolated_x(arr_3[i], interpolation);
            List<float> calculated_y = Interpolated_y(arr_3[i], arr_1[i], calculated_z);
            List<float> calculated_w = Interpolated_y(arr_3[i], arr_2[i], calculated_z);
            for (int j = 0; j < calculated_z.Count; j++)
                interpolated_x.Add(arr_4[i]);
            interpolated_y.AddRange(calculated_y);
            interpolated_w.AddRange(calculated_w);
            interpolated_z.AddRange(calculated_z);
        }

        return (interpolated_x, interpolated_y, interpolated_w, interpolated_z);
    }

    // функция для сортировки 3 массивов, используя первый массив как ключ
    public static (List<float>, List<float>, List<float>, List<float>) Sorting(
        List<float> a, List<float> b, List<float> c, List<float> d)
    {
        List<quartet> store = new List<quartet>();
        for (int i = 0; i < a.Count; i++)
            store.Add(new quartet(a[i], b[i], c[i], d[i]));

        store.Sort((first, second) => first.item_1.CompareTo(second.item_1));

        List<float> item_1 = new List<float>();
        List<float> item_2 = new List<float>();
        List<float> item_3 = new List<float>();
        List<float> item_4 = new List<float>();

        foreach (quartet item in store)
        {
            item_1.Add(item.item_1);
            item_2.Add(item.item_2);
            item_3.Add(item.item_3);
            item_4.Add(item.item_4);
        }

        return (item_1, item_2, item_3, item_4);
    }
}
