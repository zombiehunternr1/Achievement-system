using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private EventPackage _emptyPackage;
    [SerializeField] private EventPackage _singlePackage;
    [SerializeField] private EventPackage _multiPackage;
    [SerializeField] private EventPackage _doublesPackage;
    [SerializeField] private EventPackage _randomPackage;
    private void Start()
    {
        EventPackageFactory.BuildAndInvoke(_emptyPackage);
        EventPackageFactory.BuildAndInvoke(_singlePackage, "This is a test");
        EventPackageFactory.BuildAndInvoke(_multiPackage, 2412.212f, true, "Wouw this gets skipped!?");
        EventPackageFactory.BuildAndInvoke(_doublesPackage, true, false, false, true);
        EventPackageFactory.BuildAndInvoke(_randomPackage, true, 24.952f, -124.95f, 12, "Will this work?");
    }
    public void EmptyPackageEvent()
    {
        Debug.Log("This is an empty event");
    }
    public void SinglePackageEvent(EventData eventDataPackage)
    {
        Debug.Log("---------------------------");
        string stringResponds = EventPackageExtractor.ExtractData<string>(eventDataPackage);
        Debug.Log(stringResponds);
    }
    public void WhatATest(EventData eventDataPackage)
    {
        Debug.Log("---------------------------");
        bool wasSuccesful = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        float randomNumber = EventPackageExtractor.ExtractData<float>(eventDataPackage);
        Debug.Log(wasSuccesful);
        Debug.Log(randomNumber);
    }
    public void Suprise(EventData eventDataPackage)
    {
        Debug.Log("---------------------------");
        string skippedString = EventPackageExtractor.ExtractData<string>(eventDataPackage);
        Debug.Log(skippedString);
    }
    public void Doubles(EventData eventDataPackage)
    {
        Debug.Log("---------------------------");
        bool bool1 = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        bool bool2 = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        bool bool3 = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        bool bool4 = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        Debug.Log(bool1);
        Debug.Log(bool2);
        Debug.Log(bool3);
        Debug.Log(bool4);
    }
    public void RandomTest(EventData eventDataPackage)
    {
        Debug.Log("---------------------------");
        bool isValid = EventPackageExtractor.ExtractData<bool>(eventDataPackage);
        float floatValue = EventPackageExtractor.ExtractData<float>(eventDataPackage);
        float anotherFloatValue = EventPackageExtractor.ExtractData<float>(eventDataPackage);
        int intvalue = EventPackageExtractor.ExtractData<int>(eventDataPackage);
        string randomText = EventPackageExtractor.ExtractData<string>(eventDataPackage);
        Debug.Log(isValid);
        Debug.Log(floatValue);
        Debug.Log(anotherFloatValue);
        Debug.Log(intvalue);
        Debug.Log(randomText);
    }
}
