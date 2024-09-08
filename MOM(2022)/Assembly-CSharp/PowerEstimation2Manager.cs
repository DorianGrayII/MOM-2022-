using System;
using System.IO;
using System.Text;
using MHUtils;
using MHUtils.NeuralNetwork;
using MHUtils.NeuralNetwork.PowerEstimation2;
using ProtoBuf;
using UnityEngine;

public class PowerEstimation2Manager : MonoBehaviour
{
    public string sourceNetwork;

    public string startGen;

    private NNEnvironment environment;

    public int threadCount = 10;

    public float evoShare = 0.05f;

    public float evoStrength = 0.3f;

    public int generationSize = 1000;

    public Neuron.NeuronType newNetworkNeuronType = Neuron.NeuronType.TanH;

    public NeuralNetwork.Model newNetworkModel = NeuralNetwork.Model.DFF_Lerp_4;

    private void Start()
    {
        this.environment = new NNEnvironment();
    }

    private void OnDestroy()
    {
        this.environment.Destroy();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F2))
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                this.environment.StopProcessing();
                PowerEstimation2Manager.SaveCurentNetwork(this.environment.network);
                this.LoadSourceNetwork();
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                this.environment.StopProcessing();
                PowerEstimation2Manager.SaveCurentNetwork(this.environment.network);
                this.GenerateNewNetwork();
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                this.environment.StopProcessing();
                PowerEstimation2Manager.SaveCurentNetwork(this.environment.network);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F3))
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
                this.EvolveNetwork(1000000);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F4))
        {
            this.LogCurentNetwork();
        }
    }

    private void LogCurentNetwork()
    {
        if (this.environment == null || (this.environment.logResults.Count < 1 && this.environment.network == null))
        {
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (LogResults logResult in this.environment.logResults)
        {
            foreach (double log in logResult.logs)
            {
                stringBuilder.Append(log);
                stringBuilder.Append(",");
            }
            stringBuilder.AppendLine();
        }
        string text = Path.Combine(MHApplication.EXTERNAL_ASSETS, this.environment.network.GetID() + ".log");
        File.WriteAllText(text, stringBuilder.ToString());
        Debug.Log("Saved " + text);
    }

    private void EvolveNetwork(int x)
    {
        NNEnvironment nNEnvironment = this.environment;
        if (nNEnvironment != null)
        {
            NNEnvironment nNEnvironment2 = this.environment;
            nNEnvironment.StopAtGeneration(((nNEnvironment2 == null) ? null : (nNEnvironment2.network?.generation + x)).GetValueOrDefault());
        }
    }

    private void StopEvolution()
    {
        this.environment?.StopAtGeneration(-1);
    }

    private void GenerateNewNetwork()
    {
        NeuralNetwork neuralNetwork = new NeuralNetwork();
        int inputDataSize = NNUnit.GetInputDataSize();
        neuralNetwork.InitializeBlank(inputDataSize, 1, this.newNetworkModel, this.newNetworkNeuronType);
        neuralNetwork.uniqueID = global::UnityEngine.Random.Range(0, int.MaxValue);
        this.environment.network = neuralNetwork;
        this.environment.ResetEnvironment(this);
    }

    private void LoadSourceNetwork()
    {
        try
        {
            string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "NN");
            path = Path.Combine(path, this.sourceNetwork);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(path, "*.nn", SearchOption.AllDirectories);
            int num = 0;
            string text = "";
            string[] array = files;
            foreach (string text2 in array)
            {
                int num2 = Convert.ToInt32(Path.GetFileName(text2).Split('.')); //, StringSplitOptions.None)[0]);
                if (num2 > num)
                {
                    num = num2;
                    text = text2;
                }
            }
            string text3 = text;
            if (!File.Exists(text3))
            {
                return;
            }
            byte[] array2 = File.ReadAllBytes(text3);
            using (MemoryStream memoryStream = new MemoryStream(array2))
            {
                memoryStream.Write(array2, 0, array2.Length);
                memoryStream.Position = 0L;
                this.environment.network = Serializer.Deserialize<NeuralNetwork>(memoryStream);
                this.environment.ResetEnvironment(this);
                Debug.Log("Loaded " + text3 + " where latest generation is " + num);
            }
        }
        catch (Exception message)
        {
            Debug.LogError(message);
        }
    }

    public static void SaveCurentNetwork(NeuralNetwork nn)
    {
        if (nn == null)
        {
            return;
        }
        using (MemoryStream memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, nn);
            memoryStream.Position = 0L;
            int uniqueID = nn.uniqueID;
            int generation = nn.generation;
            string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "NN");
            path = Path.Combine(path, uniqueID.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string text = nn.GetErrorValue().ToString();
            string text2 = Path.Combine(path, generation + "." + text + ".nn");
            byte[] bytes = memoryStream.ToArray();
            File.WriteAllBytes(text2, bytes);
            Debug.Log("Saved " + text2);
        }
    }
}
