using UnityEngine;
using Zenject;

public class Injecter : MonoInstaller {


    [SerializeField] ServerAsk serverAsk;
    //[SerializeField] WeatherView weatherView;
    [SerializeField] Dog singleDogView;

    public override void InstallBindings() {


        Container.Bind<ServerAsk>().FromInstance(serverAsk);
        //Container.Bind<WeatherView>().FromInstance(weatherView);
        Container.Bind<Dog>().FromInstance(singleDogView);

    }


}















































