(function(){"use strict";var n={6225:function(n,e,t){var o=t(6369),r=function(){var n=this,e=n._self._c;return e("div",{attrs:{id:"app"}},[e("markdown-edit")],1)},i=[],c=function(){var n=this,e=n._self._c;return e("div",{style:{height:n.screenHeight+"px"}},[e("mavon-editor",{staticClass:"markdown-container",attrs:{configs:n.configs,toolbars:n.toolbars},on:{fullScreen:n.fullScreen,save:n.save},model:{value:n.content,callback:function(e){n.content=e},expression:"content"}})],1)},a=[],l=t(1208),u={components:{mavonEditor:l.mavonEditor},data:function(){return{content:"",html:"",configs:{},clientData:{},screenHeight:0,toolbars:{bold:!0,italic:!0,header:!0,underline:!0,strikethrough:!0,mark:!0,superscript:!0,subscript:!0,quote:!0,ol:!0,ul:!0,link:!0,imagelink:!0,code:!0,table:!0,fullscreen:!0,readmodel:!0,htmlcode:!0,help:!0,undo:!0,redo:!0,trash:!0,save:!0,navigation:!1,alignleft:!0,aligncenter:!0,alignright:!0,subfield:!0,preview:!0}}},methods:{fullScreen(n){return console.log("fullScree",n),!0},change(n,e){this.html=e},save(n,e){console.log(e);try{window.chrome.webview.hostObjects.csobj.SaveMemo(JSON.stringify(this.clientData))}catch(t){alert("error")}},loadMarkdown(){window.chrome.webview.addEventListener("message",(n=>{this.clientData=n.data,this.content=this.clientData.content}))}},mounted(){this.screenWeight=document.documentElement.clientWidth,this.screenHeight=.9*document.documentElement.clientHeight,this.loadMarkdown()}},s=u,f=t(1001),d=(0,f.Z)(s,c,a,!1,null,null,null),h=d.exports,v={name:"App",components:{MarkdownEdit:h}},p=v,m=(0,f.Z)(p,r,i,!1,null,null,null),g=m.exports;o.ZP.config.productionTip=!1,new o.ZP({render:n=>n(g)}).$mount("#app")}},e={};function t(o){var r=e[o];if(void 0!==r)return r.exports;var i=e[o]={exports:{}};return n[o].call(i.exports,i,i.exports,t),i.exports}t.m=n,function(){var n=[];t.O=function(e,o,r,i){if(!o){var c=1/0;for(s=0;s<n.length;s++){o=n[s][0],r=n[s][1],i=n[s][2];for(var a=!0,l=0;l<o.length;l++)(!1&i||c>=i)&&Object.keys(t.O).every((function(n){return t.O[n](o[l])}))?o.splice(l--,1):(a=!1,i<c&&(c=i));if(a){n.splice(s--,1);var u=r();void 0!==u&&(e=u)}}return e}i=i||0;for(var s=n.length;s>0&&n[s-1][2]>i;s--)n[s]=n[s-1];n[s]=[o,r,i]}}(),function(){t.n=function(n){var e=n&&n.__esModule?function(){return n["default"]}:function(){return n};return t.d(e,{a:e}),e}}(),function(){t.d=function(n,e){for(var o in e)t.o(e,o)&&!t.o(n,o)&&Object.defineProperty(n,o,{enumerable:!0,get:e[o]})}}(),function(){t.g=function(){if("object"===typeof globalThis)return globalThis;try{return this||new Function("return this")()}catch(n){if("object"===typeof window)return window}}()}(),function(){t.o=function(n,e){return Object.prototype.hasOwnProperty.call(n,e)}}(),function(){var n={143:0};t.O.j=function(e){return 0===n[e]};var e=function(e,o){var r,i,c=o[0],a=o[1],l=o[2],u=0;if(c.some((function(e){return 0!==n[e]}))){for(r in a)t.o(a,r)&&(t.m[r]=a[r]);if(l)var s=l(t)}for(e&&e(o);u<c.length;u++)i=c[u],t.o(n,i)&&n[i]&&n[i][0](),n[i]=0;return t.O(s)},o=self["webpackChunkmarkdown_edit"]=self["webpackChunkmarkdown_edit"]||[];o.forEach(e.bind(null,0)),o.push=e.bind(null,o.push.bind(o))}();var o=t.O(void 0,[998],(function(){return t(6225)}));o=t.O(o)})();
//# sourceMappingURL=app.6b32c9f5.js.map