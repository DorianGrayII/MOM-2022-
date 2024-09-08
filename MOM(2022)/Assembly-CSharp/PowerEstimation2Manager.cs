using MHUtils;
using MHUtils.NeuralNetwork;
using MHUtils.NeuralNetwork.PowerEstimation2;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PowerEstimation2Manager : MonoBehaviour
{
    public string sourceNetwork;
    public string startGen;
    private NNEnvironment environment;
    public int threadCount = 10;
    public float evoShare = 0.05f;
    public float evoStrength = 0.3f;
    public int generationSize = 0x3e8;
    public Neuron.NeuronType newNetworkNeuronType = Neuron.NeuronType.TanH;
    public MHUtils.NeuralNetwork.NeuralNetwork.Model newNetworkModel = MHUtils.NeuralNetwork.NeuralNetwork.Model.DFF_Lerp_4;

    private void EvolveNetwork(int x)
    {
        if (this.environment == null)
        {
            NNEnvironment environment = this.environment;
        }
        else
        {
            int? nullable;
            int? nullable3;
            if (this.environment == null)
            {
                NNEnvironment environment = this.environment;
                nullable = null;
                nullable3 = nullable;
            }
            else
            {
                int? nullable2;
                int? nullable1;
                if (this.environment.network != null)
                {
                    nullable1 = new int?(this.environment.network.generation);
                }
                else
                {
                    MHUtils.NeuralNetwork.NeuralNetwork network = this.environment.network;
                    nullable2 = null;
                    nullable1 = nullable2;
                }
                nullable = nullable1;
                int num = x;
                if (nullable != null)
                {
                    nullable3 = new int?(nullable.GetValueOrDefault() + num);
                }
                else
                {
                    nullable2 = null;
                    nullable3 = nullable2;
                }
            }
            this.environment.StopAtGeneration(nullable3.GetValueOrDefault());
        }
    }

    private void GenerateNewNetwork()
    {
        MHUtils.NeuralNetwork.NeuralNetwork network = new MHUtils.NeuralNetwork.NeuralNetwork();
        network.InitializeBlank(NNUnit.GetInputDataSize(), 1, this.newNetworkModel, this.newNetworkNeuronType);
        network.uniqueID = UnityEngine.Random.Range(0, 0x7fffffff);
        this.environment.network = network;
        this.environment.ResetEnvironment(this);
    }

    private void LoadSourceNetwork()
    {
        try
        {
            string path = Path.Combine(Path.Combine(MHApplication.EXTERNAL_ASSETS, "NN"), this.sourceNetwork);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int num = 0;
            string str2 = "";
            string[] strArray = Directory.GetFiles(path, "*.nn", SearchOption.AllDirectories);
            int index = 0;
            while (true)
            {
                if (index >= strArray.Length)
                {
                    string str3 = str2;
                    if (File.Exists(str3))
                    {
                        byte[] buffer = File.ReadAllBytes(str3);
                        using (MemoryStream stream = new MemoryStream(buffer))
                        {
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Position = 0L;
                            this.environment.network = Serializer.Deserialize<MHUtils.NeuralNetwork.NeuralNetwork>(stream);
                            this.environment.ResetEnvironment(this);
                            Debug.Log("Loaded " + str3 + " where latest generation is " + num.ToString());
                        }
                    }
                    break;
                }
                string str4 = strArray[index];
                int num3 = Convert.ToInt32(Path.GetFileName(str4).Split(".", StringSplitOptions.None)[0]);
                if (num3 > num)
                {
                    num = num3;
                    str2 = str4;
                }
                index++;
            }
        }
        catch (Exception exception1)
        {
            Debug.LogError(exception1);
        }
    }

    private void LogCurentNetwork()
    {
        if ((this.environment != null) && ((this.environment.logResults.Count >= 1) || (this.environment.network != null)))
        {
            StringBuilder builder = new StringBuilder();
            using (List<LogResults>.Enumerator enumerator = this.environment.logResults.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (double num in enumerator.Current.logs)
                    {
                        builder.Append(num);
                        builder.Append(",");
                    }
                    builder.AppendLine();
                }
            }
            string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, this.environment.network.GetID() + ".log");
            File.WriteAllText(path, builder.ToString());
            Debug.Log("Saved " + path);
        }
    }

    private void OnDestroy()
    {
        this.environment.Destroy();
    }

    public static void SaveCurentNetwork(MHUtils.NeuralNetwork.NeuralNetwork nn)
    {
        if (nn != null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<MHUtils.NeuralNetwork.NeuralNetwork>(stream, nn);
                stream.Position = 0L;
                int uniqueID = nn.uniqueID;
                int generation = nn.generation;
                string path = Path.Combine(Path.Combine(MHApplication.EXTERNAL_ASSETS, "NN"), uniqueID.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str3 = Path.Combine(path, generation.ToString() + "." + nn.GetErrorValue().ToString() + ".nn");
                File.WriteAllBytes(str3, stream.ToArray());
                Debug.Log("Saved " + str3);
            }
        }
    }

    private void Start()
    {
        this.environment = new NNEnvironment();
    }

    private void StopEvolution()
    {
        if (this.environment == null)
        {
            NNEnvironment environment = this.environment;
        }
        else
        {
            this.environment.StopAtGeneration(-1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F2))
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                this.environment.StopProcessing();
                SaveCurentNetwork(this.environment.network);
                this.LoadSourceNetwork();
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                this.environment.StopProcessing();
                SaveCurentNetwork(this.environment.network);
                this.GenerateNewNetwork();
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                this.environment.StopProcessing();
                SaveCurentNetwork(this.environment.network);
            }
        }
        else if (!Input.GetKeyUp(KeyCode.F3))
        {
            if (Input.GetKeyUp(KeyCode.F4))
            {
                this.LogCurentNetwork();
            }
        }
        else
        {
            this.StopEvolution();
            if (Input.GetKey(KeyCode.Alpha1))
            {
                Debug.Log("EvolutionStart 1");
                this.EvolveNetwork(1);
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                Debug.Log("EvolutionStart 10");
                this.EvolveNetwork(10);
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                Debug.Log("EvolutionStart Continues");
                this.EvolveNetwork(0xf4240);
            }
        }
    }
}

