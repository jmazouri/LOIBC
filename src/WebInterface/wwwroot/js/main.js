Vue.filter('prettyDate', function (value) {
    return moment(value).format('MM-DD-YY HH:mm:ss');
});

Vue.filter('camelToSpace', function (value)
{
    return value.replace(/([A-Z])/g, ' $1')
        .replace(/^./, function(str) { return str.toUpperCase(); });
});

Vue.filter('nameCleanup', function(value)
{
    return value.replace("LOIBC.SpamHeuristics.", "").replace("Heuristic", "");
});

Vue.filter('round', function(value, decimals)
{
    return value.toFixed(decimals);
});

var router = new VueRouter();

var App = Vue.extend({
    el: function () { return '#app'; },
    created: function ()
    {
        this.refreshData();
    },
    methods:
    {
        updateChannels: function(server)
        {
            fetch(new Request('/api/updateServerChannels',
            {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json'
                }),
                body: JSON.stringify(server)
            }));
        },
        refreshData: function()
        {
            var thisVue = this;

            fetch(new Request('/api/heuristics'))
            .then(function (response)
            {
                return response.json();
            })
            .then(function (data)
            {
                thisVue.Heuristics = data;
            });

            fetch(new Request('/api/serverinfo'))
            .then(function (response)
            {
                return response.json();
            })
            .then(function (data)
            {
                thisVue.DiscordServers = data;
            });

            fetch(new Request('/api/invitelink'))
            .then(function (response)
            {
                return response.text();
            })
            .then(function(data)
            {
                thisVue.InviteLink = data;
            });

            fetch(new Request('/api/logs'))
            .then(function (response)
            {
                return response.json();
            })
            .then(function (data)
            {
                _.map(data, function (entry)
                {
                    _.assign(entry,
                    {
                        showDetails: false
                    });
                });

                if (data.length !== 0)
                {
                    thisVue.Logs = _.groupBy(data, function(ele)
                    {
                        return ele.channel;
                    });
                }
                else
                {
                    thisVue.Logs = data;
                }
                
            });
        }
    },
    data: function()
    {
        return {
            Heuristics: {},
            DiscordServers: {},
            Logs: [],
            InviteLink: ""
        };
    }
   
});

router.map(
{
    '/logs':
    {
        component:
        {
            template: '#logTemplate'
        }
    },
    '/settings':
    {
        component:
        {
            template: '#settingsTemplate'
        }
    },
    '/servers':
    {
        component:
        {
            template: '#serverTemplate'
        }
    }
});

router.beforeEach(function(transition)
{
    transition.to.router.app.refreshData();
    transition.next();
});

router.start(App, '#app');