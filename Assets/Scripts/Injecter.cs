using UnityEngine;
using Zenject;

public class Injecter : MonoInstaller {


    [SerializeField] ServerAsk serverAsk;
    [SerializeField] WeatherView weatherView;


    public override void InstallBindings() {


        Container.Bind<ServerAsk>().FromInstance(serverAsk);
        Container.Bind<WeatherView>().FromInstance(weatherView);


    }


}















































