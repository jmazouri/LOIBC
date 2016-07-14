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
        this.refreshServerInfo();
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
        refreshHeuristics: function ()
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
        },
        refreshTriggers: function () {
            var thisVue = this;

            fetch(new Request('/api/triggers'))
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                thisVue.Triggers = data;
            });
        },
        refreshServerInfo: function ()
        {
            var thisVue = this;

            fetch(new Request('/api/serverinfo'))
            .then(function (response)
            {
                return response.json();
            })
            .then(function (data)
            {
                thisVue.DiscordServers = data;
            });
        },
        refreshInviteLink: function ()
        {
            var thisVue = this;

            fetch(new Request('/api/invitelink'))
            .then(function (response)
            {
                return response.text();
            })
            .then(function(data)
            {
                thisVue.InviteLink = data;
            });
        },
        refreshLogs: function ()
        {
            var thisVue = this;

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
        },
        refreshData: function()
        {
            this.refreshHeuristics();
            this.refreshTriggers();
            this.refreshInviteLink();
            this.refreshServerInfo();
            this.refreshLogs();
        }
    },
    data: function()
    {
        return {
            Heuristics: {},
            Triggers: {},
            DiscordServers: {},
            Logs: [],
            InviteLink: ""
        };
    },
    ready: function()
    {
        
    }
   
});

router.redirect({
    '*': '/logs'
});

router.map(
{
    '/logs':
    {
        component:
        {
            template: '#logTemplate',
            route:
            {
                activate: function (transition) {
                    transition.to.router.app.refreshLogs();
                    transition.next();
                }
            }
        }
    },
    '/settings':
    {
        component:
        {
            template: '#settingsTemplate',
            route:
            {
                activate: function (transition)
                {
                    transition.to.router.app.refreshHeuristics();
                    transition.to.router.app.refreshTriggers();
                    transition.next();
                }
            }
        }
    },
    '/servers':
    {
        component:
        {
            template: '#serverTemplate',
            route:
            {
                activate: function (transition)
                {
                    transition.to.router.app.refreshServerInfo();
                    transition.to.router.app.refreshInviteLink();
                    transition.next();
                }
            }
        }
    }
});

router.beforeEach(function(transition)
{
    //transition.to.router.app.refreshServerInfo();
    transition.next();
});

router.start(App, '#app');