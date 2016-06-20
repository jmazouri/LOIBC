new Vue({
    el: '#app',
    created: function () {
        var thisVue = this;

        var oReq = new XMLHttpRequest();

        fetch(new Request('/api/data'))
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                thisVue.Heuristics = _.map(data,
                    function (e) {
                        return e.replace("Heuristic", "");
                    });
            });
    },
    data:
    {
        Heuristics: {}
    }
});