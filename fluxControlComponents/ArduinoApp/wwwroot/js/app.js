const host = "http://192.168.43.139:4444";

angular.module('arduinoApp', ['ngSanitize'])

.service('historicHub', function ()
{
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(host + "/API/Historic")
        .build();
 
    connection.start().catch(err => console.error(err.toString()));
          
    return connection;
})

.controller('mainController', function ($scope, $http, $timeout, historicHub)
{
    $scope.token = null; // access Token
    $scope.rules = null; // rules config

    $scope.historic = [];

    historicHub.on('VehicleAction', (record) => 
    {
        $scope.getCompany(record.busRegistered.busCompany)

        .then((response) => 
        {
            let company = response.data;

            let alert = {

                arrival: record.arrival,
                departure: record.departure,
                type: (!record.departure) ? 'entrada' : 'saida',
                message: (!record.departure ? 'Entrada' : 'Saída') + 
                    " do veículo <strong>" + record.busRegistered.licensePlate + 
                    "</strong> da empresa <strong>" + company.name + "</strong> registrado.",
                
            }

            if (!!record.departure)
            {
                $scope.chargeRegistration(record, company)

                .then((response) => 
                {
                    let invoice = response.data;
                    
                    let arrival = moment(record.arrival);
                    let departure = moment(record.departure);

                    alert.charge = "Foram cobrados <strong>R$ " + invoice.totalCost.toFixed(2) + 
                        "</strong> da empresa <strong>" + company.name + 
                        "</strong> pela permanência de <strong>" + Number.parseInt(moment.duration(departure.diff(arrival)).asSeconds()) + 
                        " segundos</strong> do veículo <strong>" + record.busRegistered.licensePlate + "</strong>.";
                });
            }

            $scope.historic.push(alert);
            scrollTo(0, document.getElementsByTagName('body')[0].scrollHeight);

        });

    });

    historicHub.on('Warning', (warning) => 
    {
        $scope.historic.push({

            arrival: null,
            departure: null,
            moment: warning.moment,
            type: 'warning',
            message: (!warning.licensePlate) ? 
                "Não foi possível identificar a placa." : 
                "Placa <strong>" + warning.licensePlate + "</strong> não reconhecida pelo sistema.",
            charge: null
        
        });

        $scope.$apply();
        scrollTo(0, document.getElementsByTagName('body')[0].scrollHeight);
    });

    // login: 100419
    // password: 123456
    $scope.makeLogin = function (login, password)
    {
        $http.post(host + '/API/User/Login',
        { 
            registration: login, 
            password: password
        })
        .then((response) => 
        {
            $scope.token = response.data.accessToken,
            $scope.getRules();
        })
        .catch(response => console.error(response));

    };

    $scope.getRules = function ()
    {
        $http({
            method: 'GET',
            url: host + '/API/Provider/Rules/Get',
            headers: {'Authorization': 'Bearer ' + $scope.token}, 
        })
        .then(response => $scope.rules = response.data)
        .catch(response => console.error(response));
    };

    $scope.updateRules = function (tax, interval)
    {
        let rules = {
            id: 1,
            tax: tax || $scope.rules.tax,
            intervalMinutes: interval || $scope.rules.intervalMinutes
        };

        $http({
            method: 'PUT',
            url: host + '/API/Provider/Rules/Update',
            headers: {'Authorization': 'Bearer ' + $scope.token}, 
            data: rules 
        })
        .then((response) => {
            $scope.rules.message = response.data.message + "!";
            $timeout(() => {$scope.rules.message = ""}, 1500);
        })
        .catch(response => console.error(response));
    };

    $scope.getCompany = function (companyId)
    {
        try
        {
            return $http({
                method: 'GET',
                url: host + '/API/Company/Get/' + companyId,
                headers: {'Authorization': 'Bearer ' + $scope.token}, 
            });
        }

        catch (ex)
        {
            console.error(response)
        }

    };

    $scope.chargeRegistration = function (record, company)
    {
        try
        {
            return $http({
                method: 'GET',
                url: host + '/API/Invoice/ChargeRegistration/' + record.id,
                headers: {'Authorization': 'Bearer ' + $scope.token}, 
            });
        }

        catch (ex)
        {
            console.error(ex);
        }
    };

});