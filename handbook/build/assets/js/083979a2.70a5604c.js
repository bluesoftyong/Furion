"use strict";(self.webpackChunkfurion=self.webpackChunkfurion||[]).push([[6732],{3905:function(e,n,t){t.d(n,{Zo:function(){return s},kt:function(){return u}});var a=t(7294);function i(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function r(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);n&&(a=a.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,a)}return t}function o(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?r(Object(t),!0).forEach((function(n){i(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):r(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function l(e,n){if(null==e)return{};var t,a,i=function(e,n){if(null==e)return{};var t,a,i={},r=Object.keys(e);for(a=0;a<r.length;a++)t=r[a],n.indexOf(t)>=0||(i[t]=e[t]);return i}(e,n);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);for(a=0;a<r.length;a++)t=r[a],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(i[t]=e[t])}return i}var c=a.createContext({}),p=function(e){var n=a.useContext(c),t=n;return e&&(t="function"==typeof e?e(n):o(o({},n),e)),t},s=function(e){var n=p(e.components);return a.createElement(c.Provider,{value:n},e.children)},d={inlineCode:"code",wrapper:function(e){var n=e.children;return a.createElement(a.Fragment,{},n)}},m=a.forwardRef((function(e,n){var t=e.components,i=e.mdxType,r=e.originalType,c=e.parentName,s=l(e,["components","mdxType","originalType","parentName"]),m=p(t),u=i,k=m["".concat(c,".").concat(u)]||m[u]||d[u]||r;return t?a.createElement(k,o(o({ref:n},s),{},{components:t})):a.createElement(k,o({ref:n},s))}));function u(e,n){var t=arguments,i=n&&n.mdxType;if("string"==typeof e||i){var r=t.length,o=new Array(r);o[0]=m;var l={};for(var c in n)hasOwnProperty.call(n,c)&&(l[c]=n[c]);l.originalType=e,l.mdxType="string"==typeof e?e:i,o[1]=l;for(var p=2;p<r;p++)o[p]=t[p];return a.createElement.apply(null,o)}return a.createElement.apply(null,t)}m.displayName="MDXCreateElement"},1232:function(e,n,t){t.r(n),t.d(n,{assets:function(){return s},contentTitle:function(){return c},default:function(){return u},frontMatter:function(){return l},metadata:function(){return p},toc:function(){return d}});var a=t(3117),i=t(102),r=(t(7294),t(3905)),o=["components"],l={id:"cache",title:"14. \u5206\u5e03\u5f0f\u7f13\u5b58",sidebar_label:"14. \u5206\u5e03\u5f0f\u7f13\u5b58"},c=void 0,p={unversionedId:"cache",id:"cache",title:"14. \u5206\u5e03\u5f0f\u7f13\u5b58",description:"14.1 \u4ec0\u4e48\u662f\u7f13\u5b58",source:"@site/docs/cache.mdx",sourceDirName:".",slug:"/cache",permalink:"/docs/cache",draft:!1,editUrl:"https://gitee.com/dotnetchina/Furion/tree/v4/handbook/docs/cache.mdx",tags:[],version:"current",lastUpdatedBy:"\u540c\u79d1\u540c\u5b66",lastUpdatedAt:1652861433,formattedLastUpdatedAt:"May 18, 2022",frontMatter:{id:"cache",title:"14. \u5206\u5e03\u5f0f\u7f13\u5b58",sidebar_label:"14. \u5206\u5e03\u5f0f\u7f13\u5b58"},sidebar:"docs",previous:{title:"13. \u5bf9\u8c61\u6570\u636e\u6620\u5c04 Mapper",permalink:"/docs/object-mapper"},next:{title:"15. \u5b89\u5168\u9274\u6743",permalink:"/docs/auth-control"}},s={},d=[{value:"14.1 \u4ec0\u4e48\u662f\u7f13\u5b58",id:"141-\u4ec0\u4e48\u662f\u7f13\u5b58",level:2},{value:"14.2 \u7f13\u5b58\u7c7b\u578b",id:"142-\u7f13\u5b58\u7c7b\u578b",level:2},{value:"14.3 \u5185\u5b58\u7f13\u5b58\u4f7f\u7528",id:"143-\u5185\u5b58\u7f13\u5b58\u4f7f\u7528",level:2},{value:"14.3.1 \u57fa\u672c\u4f7f\u7528",id:"1431-\u57fa\u672c\u4f7f\u7528",level:3},{value:"14.3.2 \u8bbe\u7f6e\u7f13\u5b58\u9009\u9879",id:"1432-\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879",level:3},{value:"14.3.3 \u624b\u52a8\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879",id:"1433-\u624b\u52a8\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879",level:3},{value:"14.3.4 \u7f13\u5b58\u4f9d\u8d56\u5173\u7cfb",id:"1434-\u7f13\u5b58\u4f9d\u8d56\u5173\u7cfb",level:3},{value:"14.4 \u5206\u5e03\u5f0f\u7f13\u5b58",id:"144-\u5206\u5e03\u5f0f\u7f13\u5b58",level:2},{value:"14.4.1 \u4f7f\u7528\u6761\u4ef6",id:"1441-\u4f7f\u7528\u6761\u4ef6",level:3},{value:"14.4.2 <code>IDistributedCache</code>",id:"1442-idistributedcache",level:3},{value:"14.4.3 \u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58",id:"1443-\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58",level:3},{value:"14.4.4 \u5206\u5e03\u5f0f SQL Server \u7f13\u5b58",id:"1444-\u5206\u5e03\u5f0f-sql-server-\u7f13\u5b58",level:3},{value:"14.4.5 \u5206\u5e03\u5f0f <code>Redis</code> \u7f13\u5b58",id:"1445-\u5206\u5e03\u5f0f-redis-\u7f13\u5b58",level:3},{value:"14.4.6 \u5206\u5e03\u5f0f <code>NCache</code> \u7f13\u5b58",id:"1446-\u5206\u5e03\u5f0f-ncache-\u7f13\u5b58",level:3},{value:"14.5 \u5206\u5e03\u5f0f\u7f13\u5b58\u4f7f\u7528",id:"145-\u5206\u5e03\u5f0f\u7f13\u5b58\u4f7f\u7528",level:2},{value:"14.6 \u5206\u5e03\u5f0f\u7f13\u5b58\u5efa\u8bae",id:"146-\u5206\u5e03\u5f0f\u7f13\u5b58\u5efa\u8bae",level:2},{value:"14.7 \u53cd\u9988\u4e0e\u5efa\u8bae",id:"147-\u53cd\u9988\u4e0e\u5efa\u8bae",level:2}],m={toc:d};function u(e){var n=e.components,t=(0,i.Z)(e,o);return(0,r.kt)("wrapper",(0,a.Z)({},m,t,{components:n,mdxType:"MDXLayout"}),(0,r.kt)("h2",{id:"141-\u4ec0\u4e48\u662f\u7f13\u5b58"},"14.1 \u4ec0\u4e48\u662f\u7f13\u5b58"),(0,r.kt)("p",null,"\u7f13\u5b58\u53ef\u4ee5\u51cf\u5c11\u751f\u6210\u5185\u5bb9\u6240\u9700\u7684\u5de5\u4f5c\uff0c\u4ece\u800c\u663e\u8457\u63d0\u9ad8\u5e94\u7528\u7a0b\u5e8f\u7684\u6027\u80fd\u548c\u53ef\u4f38\u7f29\u6027\u3002 ",(0,r.kt)("strong",{parentName:"p"},"\u7f13\u5b58\u9002\u7528\u4e8e\u4e0d\u7ecf\u5e38\u66f4\u6539\u7684\u6570\u636e\uff0c\u56e0\u4e3a\u751f\u6210\u6210\u672c\u5f88\u9ad8"),"\u3002 \u901a\u8fc7\u7f13\u5b58\uff0c\u53ef\u6bd4\u4ece\u6570\u636e\u6e90\u8fd4\u56de\u6570\u636e\u7684\u526f\u672c\u901f\u5ea6\u5feb\u5f97\u591a\u3002 \u5e94\u8be5\u5bf9\u5e94\u7528\u8fdb\u884c\u7f16\u5199\u548c\u6d4b\u8bd5\uff0c\u4f7f\u5176\u4e0d\u8981\u6c38\u8fdc\u4f9d\u8d56\u4e8e\u7f13\u5b58\u7684\u6570\u636e\u3002"),(0,r.kt)("h2",{id:"142-\u7f13\u5b58\u7c7b\u578b"},"14.2 \u7f13\u5b58\u7c7b\u578b"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"\u5185\u5b58\u7f13\u5b58\uff1a\u987e\u540d\u601d\u4e49\uff0c\u5c31\u662f\u7f13\u5b58\u5728\u5e94\u7528\u90e8\u7f72\u6240\u5728\u670d\u52a1\u5668\u7684\u5185\u5b58\u4e2d"),(0,r.kt)("li",{parentName:"ul"},"\u5206\u5e03\u5f0f\u7f13\u5b58\uff1a\u5206\u5e03\u5f0f\u7f13\u5b58\u662f\u7531\u591a\u4e2a\u5e94\u7528\u670d\u52a1\u5668\u5171\u4eab\u7684\u7f13\u5b58"),(0,r.kt)("li",{parentName:"ul"},"\u54cd\u5e94\u7f13\u5b58\uff1a\u7f13\u5b58\u670d\u52a1\u5668\u7aef ",(0,r.kt)("inlineCode",{parentName:"li"},"Not Modified")," \u7684\u6570\u636e")),(0,r.kt)("h2",{id:"143-\u5185\u5b58\u7f13\u5b58\u4f7f\u7528"},"14.3 \u5185\u5b58\u7f13\u5b58\u4f7f\u7528"),(0,r.kt)("p",null,"\u5185\u5b58\u7f13\u5b58\u662f\u6700\u5e38\u7528\u7684\u7f13\u5b58\u65b9\u5f0f\uff0c\u5177\u6709\u5b58\u53d6\u5feb\uff0c\u6548\u7387\u9ad8\u7279\u70b9\u3002"),(0,r.kt)("p",null,"\u5185\u5b58\u7f13\u5b58\u901a\u8fc7\u6ce8\u5165 ",(0,r.kt)("inlineCode",{parentName:"p"},"IMemoryCache")," \u65b9\u5f0f\u6ce8\u5165\u5373\u53ef\u3002"),(0,r.kt)("admonition",{title:"\u5907\u6ce8",type:"important"},(0,r.kt)("p",{parentName:"admonition"},"\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},"Furion")," \u6846\u67b6\u4e2d\uff0c\u5185\u5b58\u7f13\u5b58\u670d\u52a1\u5df2\u7ecf\u9ed8\u8ba4\u6ce8\u518c\uff0c\u65e0\u9700\u624b\u52a8\u6ce8\u518c\u3002")),(0,r.kt)("h3",{id:"1431-\u57fa\u672c\u4f7f\u7528"},"14.3.1 \u57fa\u672c\u4f7f\u7528"),(0,r.kt)("p",null,"\u5982\uff0c\u7f13\u5b58\u5f53\u524d\u65f6\u95f4\uff1a"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers  {13,21-24}",showLineNumbers:!0,"":!0,"{13,21-24}":!0},'using Furion.DynamicApiController;\nusing Microsoft.Extensions.Caching.Memory;\nusing System;\n\nnamespace Furion.Application\n{\n    public class CacheServices : IDynamicApiController\n    {\n        private const string _timeCacheKey = "cache_time";\n\n        private readonly IMemoryCache _memoryCache;\n\n        public CacheServices(IMemoryCache memoryCache)\n        {\n            _memoryCache = memoryCache;\n        }\n\n        [ApiDescriptionSettings(KeepName = true)]\n        public DateTimeOffset GetOrCreate()\n        {\n            return _memoryCache.GetOrCreate(_timeCacheKey, entry =>\n            {\n                return DateTimeOffset.UtcNow;\n            });\n        }\n    }\n}\n')),(0,r.kt)("h3",{id:"1432-\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879"},"14.3.2 \u8bbe\u7f6e\u7f13\u5b58\u9009\u9879"),(0,r.kt)("p",null,"\u5185\u5b58\u7f13\u5b58\u652f\u6301\u8bbe\u7f6e\u7f13\u5b58\u65f6\u95f4\u3001\u7f13\u5b58\u5927\u5c0f\u3001\u53ca\u7edd\u5bf9\u7f13\u5b58\u8fc7\u671f\u65f6\u95f4\u7b49"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},"_memoryCache.GetOrCreate(_timeCacheKey, entry =>\n{\n    entry.SlidingExpiration = TimeSpan.FromSeconds(3);  // \u6ed1\u52a8\u7f13\u5b58\u65f6\u95f4\n    return DateTimeOffset.UtcNow;\n});\n\nawait _memoryCache.GetOrCreateAsync(_timeCacheKey, async entry =>\n{\n    // \u8fd9\u91cc\u53ef\u4ee5\u4f7f\u7528\u5f02\u6b65~~\n});\n")),(0,r.kt)("admonition",{title:"\u5173\u4e8e\u7f13\u5b58\u65f6\u95f4",type:"caution"},(0,r.kt)("p",{parentName:"admonition"},"\u5177\u6709\u53ef\u8c03\u8fc7\u671f\u7684\u7f13\u5b58\u9879\u96c6\u5b58\u5728\u8fc7\u65f6\u7684\u98ce\u9669\u3002 \u5982\u679c\u8bbf\u95ee\u7684\u65f6\u95f4\u6bd4\u6ed1\u52a8\u8fc7\u671f\u65f6\u95f4\u95f4\u9694\u66f4\u9891\u7e41\uff0c\u5219\u8be5\u9879\u5c06\u6c38\u4e0d\u8fc7\u671f\u3002"),(0,r.kt)("p",{parentName:"admonition"},"\u5c06\u5f39\u6027\u8fc7\u671f\u4e0e\u7edd\u5bf9\u8fc7\u671f\u7ec4\u5408\u5728\u4e00\u8d77\uff0c\u4ee5\u4fdd\u8bc1\u9879\u76ee\u5728\u5176\u7edd\u5bf9\u8fc7\u671f\u65f6\u95f4\u901a\u8fc7\u540e\u8fc7\u671f\u3002 \u7edd\u5bf9\u8fc7\u671f\u4f1a\u5c06\u9879\u7684\u4e0a\u9650\u8bbe\u7f6e\u4e3a\u53ef\u7f13\u5b58\u9879\u7684\u65f6\u95f4\uff0c\u540c\u65f6\u4ecd\u5141\u8bb8\u9879\u5728\u53ef\u8c03\u6574\u8fc7\u671f\u65f6\u95f4\u95f4\u9694\u5185\u672a\u8bf7\u6c42\u65f6\u63d0\u524d\u8fc7\u671f\u3002"),(0,r.kt)("p",{parentName:"admonition"},"\u5982\u679c\u540c\u65f6\u6307\u5b9a\u4e86\u7edd\u5bf9\u8fc7\u671f\u548c\u53ef\u8c03\u8fc7\u671f\u65f6\u95f4\uff0c\u5219\u8fc7\u671f\u65f6\u95f4\u4ee5\u903b\u8f91\u65b9\u5f0f\u8fd0\u7b97\u3002 \u5982\u679c\u6ed1\u52a8\u8fc7\u671f\u65f6\u95f4\u95f4\u9694 \u6216 \u7edd\u5bf9\u8fc7\u671f\u65f6\u95f4\u901a\u8fc7\uff0c\u5219\u4ece\u7f13\u5b58\u4e2d\u9010\u51fa\u8be5\u9879\u3002"),(0,r.kt)("p",{parentName:"admonition"},"\u5982\uff1a"),(0,r.kt)("pre",{parentName:"admonition"},(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},"_memoryCache.GetOrCreate(_timeCacheKey, entry =>\n{\n    entry.SetSlidingExpiration(TimeSpan.FromSeconds(3));\n    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);\n    return DateTime.Now;\n});\n")),(0,r.kt)("p",{parentName:"admonition"},"\u524d\u9762\u7684\u4ee3\u7801\u4fdd\u8bc1\u6570\u636e\u7684\u7f13\u5b58\u65f6\u95f4\u4e0d\u8d85\u8fc7\u7edd\u5bf9\u65f6\u95f4\u3002")),(0,r.kt)("h3",{id:"1433-\u624b\u52a8\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879"},"14.3.3 \u624b\u52a8\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879"),(0,r.kt)("p",null,"\u9664\u4e86\u4e0a\u9762\u7684 ",(0,r.kt)("inlineCode",{parentName:"p"},"Func<MemoryCacheEntryOptions, object>")," \u65b9\u5f0f\u8bbe\u7f6e\u7f13\u5b58\u9009\u9879\uff0c\u6211\u4eec\u53ef\u4ee5\u624b\u52a8\u521b\u5efa\u5e76\u8bbe\u7f6e\uff0c\u5982\uff1a"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},"var cacheEntryOptions = new MemoryCacheEntryOptions()\n                .SetSlidingExpiration(TimeSpan.FromSeconds(3));\n\n_memoryCache.Set(_timeCacheKey, DateTimeOffset.UtcNow, cacheEntryOptions);\n")),(0,r.kt)("h3",{id:"1434-\u7f13\u5b58\u4f9d\u8d56\u5173\u7cfb"},"14.3.4 \u7f13\u5b58\u4f9d\u8d56\u5173\u7cfb"),(0,r.kt)("p",null,"\u4e0b\u9762\u7684\u793a\u4f8b\u6f14\u793a\u5982\u4f55\u5728\u4f9d\u8d56\u6761\u76ee\u8fc7\u671f\u540e\u4f7f\u7f13\u5b58\u6761\u76ee\u8fc7\u671f\u3002 ",(0,r.kt)("inlineCode",{parentName:"p"},"CancellationChangeToken")," \u6dfb\u52a0\u5230\u7f13\u5b58\u7684\u9879\u3002 \u5f53 ",(0,r.kt)("inlineCode",{parentName:"p"},"Cancel")," \u5728\u4e0a\u8c03\u7528\u65f6 ",(0,r.kt)("inlineCode",{parentName:"p"},"CancellationTokenSource")," \uff0c\u5c06\u9010\u51fa\u4e24\u4e2a\u7f13\u5b58\u9879\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},'public IActionResult CreateDependentEntries()\n{\n    var cts = new CancellationTokenSource();\n    _cache.Set(CacheKeys.DependentCTS, cts);\n\n    using (var entry = _cache.CreateEntry(CacheKeys.Parent))\n    {\n        // expire this entry if the dependant entry expires.\n        entry.Value = DateTime.Now;\n        entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);\n\n        _cache.Set(CacheKeys.Child,\n            DateTime.Now,\n            new CancellationChangeToken(cts.Token));\n    }\n\n    return RedirectToAction("GetDependentEntries");\n}\n\npublic IActionResult GetDependentEntries()\n{\n    return View("Dependent", new DependentViewModel\n    {\n        ParentCachedTime = _cache.Get<DateTime?>(CacheKeys.Parent),\n        ChildCachedTime = _cache.Get<DateTime?>(CacheKeys.Child),\n        Message = _cache.Get<string>(CacheKeys.DependentMessage)\n    });\n}\n\npublic IActionResult RemoveChildEntry()\n{\n    _cache.Get<CancellationTokenSource>(CacheKeys.DependentCTS).Cancel();\n    return RedirectToAction("GetDependentEntries");\n}\n\nprivate static void DependentEvictionCallback(object key, object value,\n    EvictionReason reason, object state)\n{\n    var message = $"Parent entry was evicted. Reason: {reason}.";\n    ((HomeController)state)._cache.Set(CacheKeys.DependentMessage, message);\n}\n')),(0,r.kt)("p",null,"\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"CancellationTokenSource")," \u5141\u8bb8\u5c06\u591a\u4e2a\u7f13\u5b58\u6761\u76ee\u4f5c\u4e3a\u4e00\u4e2a\u7ec4\u9010\u51fa\u3002 ",(0,r.kt)("inlineCode",{parentName:"p"},"using")," \u5728\u4e0a\u9762\u7684\u4ee3\u7801\u4e2d\uff0c\u5728\u5757\u4e2d\u521b\u5efa\u7684\u7f13\u5b58\u6761\u76ee ",(0,r.kt)("inlineCode",{parentName:"p"},"using")," \u5c06\u7ee7\u627f\u89e6\u53d1\u5668\u548c\u8fc7\u671f\u8bbe\u7f6e\u3002"),(0,r.kt)("admonition",{title:"\u4e86\u89e3\u66f4\u591a",type:"note"},(0,r.kt)("p",{parentName:"admonition"},"\u60f3\u4e86\u89e3\u66f4\u591a ",(0,r.kt)("inlineCode",{parentName:"p"},"\u5185\u5b58\u4e2d\u7684\u7f13\u5b58")," \u77e5\u8bc6\u53ef\u67e5\u9605 ",(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/zh-cn/aspnet/core/performance/caching/memory?view=aspnetcore-5.0"},"ASP.NET Core - \u5185\u5b58\u7f13\u5b58")," \u7ae0\u8282\u3002")),(0,r.kt)("h2",{id:"144-\u5206\u5e03\u5f0f\u7f13\u5b58"},"14.4 \u5206\u5e03\u5f0f\u7f13\u5b58"),(0,r.kt)("p",null,"\u5206\u5e03\u5f0f\u7f13\u5b58\u662f\u7531\u591a\u4e2a\u5e94\u7528\u670d\u52a1\u5668\u5171\u4eab\u7684\u7f13\u5b58\uff0c\u901a\u5e38\u4f5c\u4e3a\u5916\u90e8\u670d\u52a1\u5728\u8bbf\u95ee\u5b83\u7684\u5e94\u7528\u670d\u52a1\u5668\u4e0a\u7ef4\u62a4\u3002 \u5206\u5e03\u5f0f\u7f13\u5b58\u53ef\u4ee5\u63d0\u9ad8 ",(0,r.kt)("inlineCode",{parentName:"p"},"ASP.NET Core")," \u5e94\u7528\u7a0b\u5e8f\u7684\u6027\u80fd\u548c\u53ef\u4f38\u7f29\u6027\uff0c\u5c24\u5176\u662f\u5728\u5e94\u7528\u7a0b\u5e8f\u7531\u4e91\u670d\u52a1\u6216\u670d\u52a1\u5668\u573a\u6258\u7ba1\u65f6\u3002"),(0,r.kt)("p",null,"\u4e0e\u5176\u4ed6\u7f13\u5b58\u65b9\u6848\u76f8\u6bd4\uff0c\u5206\u5e03\u5f0f\u7f13\u5b58\u5177\u6709\u591a\u9879\u4f18\u52bf\uff0c\u5176\u4e2d\u7f13\u5b58\u7684\u6570\u636e\u5b58\u50a8\u5728\u5355\u4e2a\u5e94\u7528\u670d\u52a1\u5668\u4e0a\u3002"),(0,r.kt)("p",null,"\u5f53\u5206\u5e03\u5f0f\u7f13\u5b58\u6570\u636e\u65f6\uff0c\u6570\u636e\u5c06\uff1a"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"(\u4e00\u81f4\u6027) \u8de8\u591a\u4e2a \u670d\u52a1\u5668\u7684\u8bf7\u6c42"),(0,r.kt)("li",{parentName:"ul"},"\u5b58\u6d3b\u5728\u670d\u52a1\u5668\u91cd\u542f\u548c\u5e94\u7528\u90e8\u7f72\u4e4b\u95f4"),(0,r.kt)("li",{parentName:"ul"},"\u4e0d\u4f7f\u7528\u672c\u5730\u5185\u5b58")),(0,r.kt)("p",null,"\u5206\u5e03\u5f0f\u7f13\u5b58\u914d\u7f6e\u662f\u7279\u5b9a\u4e8e\u5b9e\u73b0\u7684\u3002 \u672c\u6587\u4ecb\u7ecd\u5982\u4f55\u914d\u7f6e ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u548c ",(0,r.kt)("inlineCode",{parentName:"p"},"Redis")," \u5206\u5e03\u5f0f\u7f13\u5b58\u3002 \u7b2c\u4e09\u65b9\u5b9e\u73b0\u4e5f\u53ef\u7528\uff0c\u4f8b\u5982 GitHub \u4e0a\u7684 ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/Alachisoft/NCache"},"NCache")," (NCache) \u3002"),(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"\u65e0\u8bba\u9009\u62e9\u54ea\u79cd\u5b9e\u73b0\uff0c\u5e94\u7528\u90fd\u4f1a\u4f7f\u7528\u63a5\u53e3\u4e0e\u7f13\u5b58\u4ea4\u4e92 ",(0,r.kt)("inlineCode",{parentName:"strong"},"IDistributedCache")," \u3002")),(0,r.kt)("h3",{id:"1441-\u4f7f\u7528\u6761\u4ef6"},"14.4.1 \u4f7f\u7528\u6761\u4ef6"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"\u82e5\u8981\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"li"},"SQL Server")," \u5206\u5e03\u5f0f\u7f13\u5b58\uff0c\u5219\u6dfb\u52a0 ",(0,r.kt)("inlineCode",{parentName:"li"},"Microsoft.Extensions.Caching.SqlServer")," \u5305"),(0,r.kt)("li",{parentName:"ul"},"\u82e5\u8981\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"li"},"Redis")," \u5206\u5e03\u5f0f\u7f13\u5b58\uff0c\u5219\u6dfb\u52a0 ",(0,r.kt)("inlineCode",{parentName:"li"},"Microsoft.Extensions.Caching.StackExchangeRedis")," \u5305"),(0,r.kt)("li",{parentName:"ul"},"\u82e5\u8981\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"li"},"NCache")," \u5206\u5e03\u5f0f\u7f13\u5b58\uff0c\u5219\u6dfb\u52a0 ",(0,r.kt)("inlineCode",{parentName:"li"},"NCache.Microsoft.Extensions.Caching.OpenSource")," \u5305")),(0,r.kt)("h3",{id:"1442-idistributedcache"},"14.4.2 ",(0,r.kt)("inlineCode",{parentName:"h3"},"IDistributedCache")),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"IDistributedCache")," \u63a5\u53e3\u63d0\u4f9b\u4ee5\u4e0b\u65b9\u6cd5\u6765\u5904\u7406\u5206\u5e03\u5f0f\u7f13\u5b58\u5b9e\u73b0\u4e2d\u7684\u9879\uff1a"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"Get/GetAsync"),"\uff1a\u63a5\u53d7\u5b57\u7b26\u4e32\u952e\uff0c\u5e76\u68c0\u7d22\u7f13\u5b58\u9879\u4f5c\u4e3a ",(0,r.kt)("inlineCode",{parentName:"li"},"byte[]")," \u6570\u7ec4\uff08\u5982\u679c\u5728\u7f13\u5b58\u4e2d\u627e\u5230\uff09"),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"Set/SetAsync"),"\uff1a\u4f7f\u7528\u5b57\u7b26\u4e32\u952e\u5c06\u9879 (\u4f5c\u4e3a ",(0,r.kt)("inlineCode",{parentName:"li"},"byte[]")," \u6570\u7ec4) \u6dfb\u52a0\u5230\u7f13\u5b58\u4e2d"),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"Refresh/RefreshAsync")," \uff1a\u6839\u636e\u9879\u7684\u952e\u5237\u65b0\u7f13\u5b58\u4e2d\u7684\u9879\uff0c\u91cd\u7f6e\u5176\u6ed1\u52a8\u8fc7\u671f\u8d85\u65f6\uff08\u5982\u679c\u6709\uff09"),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("inlineCode",{parentName:"li"},"Remove/RemoveAsync"),"\uff1a\u6839\u636e\u7f13\u5b58\u9879\u7684\u5b57\u7b26\u4e32\u952e\u5220\u9664\u7f13\u5b58\u9879")),(0,r.kt)("h3",{id:"1443-\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58"},"14.4.3 \u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58"),(0,r.kt)("p",null,"\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58\uff08",(0,r.kt)("inlineCode",{parentName:"p"},"AddDistributedMemoryCache"),"\uff09\u662f\u4e00\u4e2a\u6846\u67b6\u63d0\u4f9b\u7684\u5b9e\u73b0 ",(0,r.kt)("inlineCode",{parentName:"p"},"IDistributedCache")," \uff0c\u5b83\u5c06\u9879\u5b58\u50a8\u5728\u5185\u5b58\u4e2d\u3002 ",(0,r.kt)("strong",{parentName:"p"},"\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58\u4e0d\u662f\u5b9e\u9645\u7684\u5206\u5e03\u5f0f\u7f13\u5b58\uff0c\u7f13\u5b58\u9879\u7531\u5e94\u7528\u7a0b\u5e8f\u5b9e\u4f8b\u5b58\u50a8\u5728\u8fd0\u884c\u5e94\u7528\u7a0b\u5e8f\u7684\u670d\u52a1\u5668\u4e0a\u3002")),(0,r.kt)("p",null,"\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58\u4f18\u70b9\uff1a"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"\u7528\u4e8e\u5f00\u53d1\u548c\u6d4b\u8bd5\u65b9\u6848\u3002"),(0,r.kt)("li",{parentName:"ul"},"\u5728\u751f\u4ea7\u73af\u5883\u4e2d\u4f7f\u7528\u5355\u4e00\u670d\u52a1\u5668\u5e76\u4e14\u5185\u5b58\u6d88\u8017\u4e0d\u662f\u95ee\u9898\u3002 \u5b9e\u73b0\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58\u4f1a\u62bd\u8c61\u5316\u7f13\u5b58\u7684\u6570\u636e\u5b58\u50a8\u3002 \u5982\u679c\u9700\u8981\u591a\u4e2a\u8282\u70b9\u6216\u5bb9\u9519\uff0c\u53ef\u4ee5\u5728\u5c06\u6765\u5b9e\u73b0\u771f\u6b63\u7684\u5206\u5e03\u5f0f\u7f13\u5b58\u89e3\u51b3\u65b9\u6848\u3002")),(0,r.kt)("admonition",{title:"\u5907\u6ce8",type:"important"},(0,r.kt)("p",{parentName:"admonition"},"\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},"Furion")," \u6846\u67b6\u4e2d\uff0c\u5206\u5e03\u5f0f\u5185\u5b58\u7f13\u5b58\u670d\u52a1\u5df2\u7ecf\u9ed8\u8ba4\u6ce8\u518c\uff0c\u65e0\u9700\u624b\u52a8\u8c03\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"services.AddDistributedMemoryCache();")," \u6ce8\u518c\u3002")),(0,r.kt)("h3",{id:"1444-\u5206\u5e03\u5f0f-sql-server-\u7f13\u5b58"},"14.4.4 \u5206\u5e03\u5f0f SQL Server \u7f13\u5b58"),(0,r.kt)("p",null,"\u5206\u5e03\u5f0f ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u7f13\u5b58\u5b9e\u73b0 (",(0,r.kt)("inlineCode",{parentName:"p"},"AddDistributedSqlServerCache"),") \u5141\u8bb8\u5206\u5e03\u5f0f\u7f13\u5b58\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u6570\u636e\u5e93\u4f5c\u4e3a\u5176\u540e\u5907\u5b58\u50a8\u3002"),(0,r.kt)("p",null,"\u82e5\u8981\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u5b9e\u4f8b\u4e2d\u521b\u5efa ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u7f13\u5b58\u7684\u9879\u8868\uff0c\u53ef\u4ee5\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"sql-cache")," \u5de5\u5177\u3002 \u8be5\u5de5\u5177\u5c06\u521b\u5efa\u4e00\u4e2a\u8868\uff0c\u5176\u4e2d\u5305\u542b\u6307\u5b9a\u7684\u540d\u79f0\u548c\u67b6\u6784\u3002"),(0,r.kt)("p",null,"\u901a\u8fc7\u8fd0\u884c\u547d\u4ee4 ",(0,r.kt)("inlineCode",{parentName:"p"},"sql-cache create")," \u521b\u5efa\u4e00\u4e2a\u8868\uff0c\u63d0\u4f9b ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u5b9e\u4f8b (Data Source) \u3001\u6570\u636e\u5e93 (Initial Catalog) \u3001\u67b6\u6784 (\u4f8b\u5982) dbo \u548c\u8868\u540d\u79f0\u3002\u4f8b\u5982 ",(0,r.kt)("inlineCode",{parentName:"p"},"TestCache"),"\uff1a"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-shell",metastring:"showLineNumbers",showLineNumbers:!0},'dotnet sql-cache create "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DistCache;Integrated Security=True;" dbo TestCache\n')),(0,r.kt)("p",null,"\u521b\u5efa\u6210\u529f\u540e\uff0c\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},"Startup.cs")," \u4e2d\u6ce8\u518c\u5373\u53ef\uff1a"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},'services.AddDistributedSqlServerCache(options =>\n{\n    options.ConnectionString =\n        _config["DistCache_ConnectionString"];\n    options.SchemaName = "dbo";\n    options.TableName = "TestCache";\n});\n')),(0,r.kt)("h3",{id:"1445-\u5206\u5e03\u5f0f-redis-\u7f13\u5b58"},"14.4.5 \u5206\u5e03\u5f0f ",(0,r.kt)("inlineCode",{parentName:"h3"},"Redis")," \u7f13\u5b58"),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"Redis")," \u662f\u5185\u5b58\u4e2d\u6570\u636e\u5b58\u50a8\u7684\u5f00\u6e90\u6570\u636e\u5b58\u50a8\uff0c\u901a\u5e38\u7528\u4f5c\u5206\u5e03\u5f0f\u7f13\u5b58\u3002\u5728\u4f7f\u7528\u65f6\u901a\u8fc7 ",(0,r.kt)("inlineCode",{parentName:"p"},"services.AddStackExchangeRedisCache()")," \u4e2d\u6ce8\u518c\u5373\u53ef\u3002"),(0,r.kt)("p",null,"\u8fd9\u91cc\u4e0d\u7ec6\u8bb2 ",(0,r.kt)("inlineCode",{parentName:"p"},"Redis")," \u76f8\u5173\u5185\u5bb9\uff0c\u540e\u7eed\u7ae0\u8282\u4f1a\u4f7f\u7528\u57fa\u672c\u4f8b\u5b50\u6f14\u793a\u3002"),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"Redis")," \u57fa\u672c\u914d\u7f6e\uff1a"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},'services.AddStackExchangeRedisCache(options =>\n{\n    // \u8fde\u63a5\u5b57\u7b26\u4e32\uff0c\u8fd9\u91cc\u4e5f\u53ef\u4ee5\u8bfb\u53d6\u914d\u7f6e\u6587\u4ef6\n    options.Configuration = "192.168.111.134,password=aW1HAyupRKmiZn3Q";\n    // \u952e\u540d\u524d\u7f00\n    options.InstanceName = "furion_";\n});\n')),(0,r.kt)("h3",{id:"1446-\u5206\u5e03\u5f0f-ncache-\u7f13\u5b58"},"14.4.6 \u5206\u5e03\u5f0f ",(0,r.kt)("inlineCode",{parentName:"h3"},"NCache")," \u7f13\u5b58"),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"NCache")," \u662f\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},".NET")," \u548c ",(0,r.kt)("inlineCode",{parentName:"p"},".Net Core")," \u4e2d\u4ee5\u672c\u673a\u65b9\u5f0f\u5f00\u53d1\u7684\u5f00\u6e90\u5185\u5b58\u4e2d\u5206\u5e03\u5f0f\u7f13\u5b58\u3002 ",(0,r.kt)("inlineCode",{parentName:"p"},"NCache")," \u5728\u672c\u5730\u5de5\u4f5c\u5e76\u914d\u7f6e\u4e3a\u5206\u5e03\u5f0f\u7f13\u5b58\u7fa4\u96c6\uff0c\u9002\u7528\u4e8e\u5728 ",(0,r.kt)("inlineCode",{parentName:"p"},"Azure")," \u6216\u5176\u4ed6\u6258\u7ba1\u5e73\u53f0\u4e0a\u8fd0\u884c\u7684 ",(0,r.kt)("inlineCode",{parentName:"p"},"ASP.NET Core")," \u5e94\u7528\u3002\n\u82e5\u8981\u5728\u672c\u5730\u8ba1\u7b97\u673a\u4e0a\u5b89\u88c5\u548c\u914d\u7f6e ",(0,r.kt)("inlineCode",{parentName:"p"},"NCache"),"\uff0c\u8bf7\u53c2\u9605 ",(0,r.kt)("a",{parentName:"p",href:"https://www.alachisoft.com/resources/docs/ncache-oss/getting-started-guide-windows/"},"\u9002\u7528\u4e8e Windows \u7684 NCache \u5165\u95e8\u6307\u5357"),"\u3002"),(0,r.kt)("p",null,(0,r.kt)("inlineCode",{parentName:"p"},"NCache")," \u57fa\u672c\u914d\u7f6e\uff1a"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"\u5b89\u88c5 ",(0,r.kt)("inlineCode",{parentName:"li"},"Alachisoft.NCache.OpenSource.SDK")," \u5305"),(0,r.kt)("li",{parentName:"ul"},"\u5728 ",(0,r.kt)("a",{parentName:"li",href:"https://www.alachisoft.com/resources/docs/ncache-oss/admin-guide/client-config.html"},"ncconf")," \u4e2d\u914d\u7f6e\u7f13\u5b58\u7fa4\u96c6"),(0,r.kt)("li",{parentName:"ul"},"\u6ce8\u518c ",(0,r.kt)("inlineCode",{parentName:"li"},"NCache")," \u670d\u52a1")),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers",showLineNumbers:!0},'services.AddNCacheDistributedCache(configuration =>\n{\n    configuration.CacheName = "demoClusteredCache";\n    configuration.EnableLogs = true;\n    configuration.ExceptionsEnabled = true;\n});\n')),(0,r.kt)("h2",{id:"145-\u5206\u5e03\u5f0f\u7f13\u5b58\u4f7f\u7528"},"14.5 \u5206\u5e03\u5f0f\u7f13\u5b58\u4f7f\u7528"),(0,r.kt)("p",null,"\u82e5\u8981\u4f7f\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"IDistributedCache")," \u63a5\u53e3\uff0c\u8bf7 ",(0,r.kt)("inlineCode",{parentName:"p"},"IDistributedCache")," \u901a\u8fc7\u6784\u9020\u51fd\u6570\u4f9d\u8d56\u5173\u7cfb\u6ce8\u5165\u3002"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre",className:"language-cs",metastring:"showLineNumbers  {5,16,30-33}",showLineNumbers:!0,"":!0,"{5,16,30-33}":!0},'public class IndexModel : PageModel\n{\n    private readonly IDistributedCache _cache;\n\n    public IndexModel(IDistributedCache cache)\n    {\n        _cache = cache;\n    }\n\n    public string CachedTimeUTC { get; set; }\n\n    public async Task OnGetAsync()\n    {\n        CachedTimeUTC = "Cached Time Expired";\n        // \u83b7\u53d6\u5206\u5e03\u5f0f\u7f13\u5b58\n        var encodedCachedTimeUTC = await _cache.GetAsync("cachedTimeUTC");\n\n        if (encodedCachedTimeUTC != null)\n        {\n            CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);\n        }\n    }\n\n    public async Task<IActionResult> OnPostResetCachedTime()\n    {\n        var currentTimeUTC = DateTime.UtcNow.ToString();\n        byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);\n\n        // \u8bbe\u7f6e\u5206\u5e03\u5f0f\u7f13\u5b58\n        var options = new DistributedCacheEntryOptions()\n            .SetSlidingExpiration(TimeSpan.FromSeconds(20));\n\n        await _cache.SetAsync("cachedTimeUTC", encodedCurrentTimeUTC, options);\n\n        return RedirectToPage();\n    }\n}\n')),(0,r.kt)("h2",{id:"146-\u5206\u5e03\u5f0f\u7f13\u5b58\u5efa\u8bae"},"14.6 \u5206\u5e03\u5f0f\u7f13\u5b58\u5efa\u8bae"),(0,r.kt)("p",null,"\u786e\u5b9a ",(0,r.kt)("inlineCode",{parentName:"p"},"IDistributedCache")," \u6700\u9002\u5408\u4f60\u7684\u5e94\u7528\u7684\u5b9e\u73b0\u65f6\uff0c\u8bf7\u8003\u8651\u4ee5\u4e0b\u4e8b\u9879\uff1a"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"\u73b0\u6709\u57fa\u7840\u7ed3\u6784"),(0,r.kt)("li",{parentName:"ul"},"\u6027\u80fd\u8981\u6c42"),(0,r.kt)("li",{parentName:"ul"},"\u6210\u672c"),(0,r.kt)("li",{parentName:"ul"},"\u56e2\u961f\u7ecf\u9a8c")),(0,r.kt)("p",null,"\u7f13\u5b58\u89e3\u51b3\u65b9\u6848\u901a\u5e38\u4f9d\u8d56\u4e8e\u5185\u5b58\u4e2d\u7684\u5b58\u50a8\u4ee5\u5feb\u901f\u68c0\u7d22\u7f13\u5b58\u7684\u6570\u636e\uff0c\u4f46\u662f\uff0c\u5185\u5b58\u662f\u6709\u9650\u7684\u8d44\u6e90\uff0c\u5e76\u4e14\u5f88\u6602\u8d35\u3002 \u4ec5\u5c06\u5e38\u7528\u6570\u636e\u5b58\u50a8\u5728\u7f13\u5b58\u4e2d\u3002"),(0,r.kt)("p",null,"\u901a\u5e38\uff0c",(0,r.kt)("strong",{parentName:"p"},(0,r.kt)("inlineCode",{parentName:"strong"},"Redis")," \u7f13\u5b58\u63d0\u4f9b\u6bd4 ",(0,r.kt)("inlineCode",{parentName:"strong"},"SQL Server")," \u7f13\u5b58\u66f4\u9ad8\u7684\u541e\u5410\u91cf\u548c\u66f4\u4f4e\u7684\u5ef6\u8fdf\u3002")," \u4f46\u662f\uff0c\u901a\u5e38\u9700\u8981\u8fdb\u884c\u57fa\u51c6\u6d4b\u8bd5\u6765\u786e\u5b9a\u7f13\u5b58\u7b56\u7565\u7684\u6027\u80fd\u7279\u5f81\u3002"),(0,r.kt)("p",null,"\u5f53 ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u7528\u4f5c\u5206\u5e03\u5f0f\u7f13\u5b58\u540e\u5907\u5b58\u50a8\u65f6\uff0c\u5bf9\u7f13\u5b58\u4f7f\u7528\u540c\u4e00\u6570\u636e\u5e93\uff0c\u5e76\u4e14\u5e94\u7528\u7684\u666e\u901a\u6570\u636e\u5b58\u50a8\u548c\u68c0\u7d22\u4f1a\u5bf9\u8fd9\u4e24\u79cd\u60c5\u51b5\u7684\u6027\u80fd\u4ea7\u751f\u8d1f\u9762\u5f71\u54cd\u3002 \u5efa\u8bae\u4f7f\u7528\u5206\u5e03\u5f0f\u7f13\u5b58\u540e\u5907\u5b58\u50a8\u7684\u4e13\u7528 ",(0,r.kt)("inlineCode",{parentName:"p"},"SQL Server")," \u5b9e\u4f8b\u3002"),(0,r.kt)("h2",{id:"147-\u53cd\u9988\u4e0e\u5efa\u8bae"},"14.7 \u53cd\u9988\u4e0e\u5efa\u8bae"),(0,r.kt)("admonition",{title:"\u4e0e\u6211\u4eec\u4ea4\u6d41",type:"note"},(0,r.kt)("p",{parentName:"admonition"},"\u7ed9 Furion \u63d0 ",(0,r.kt)("a",{parentName:"p",href:"https://gitee.com/dotnetchina/Furion/issues/new?issue"},"Issue"),"\u3002")),(0,r.kt)("hr",null),(0,r.kt)("admonition",{title:"\u4e86\u89e3\u66f4\u591a",type:"note"},(0,r.kt)("p",{parentName:"admonition"},"\u60f3\u4e86\u89e3\u66f4\u591a ",(0,r.kt)("inlineCode",{parentName:"p"},"\u5206\u5e03\u5f0f\u7f13\u5b58")," \u77e5\u8bc6\u53ef\u67e5\u9605 ",(0,r.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/zh-cn/aspnet/core/performance/caching/distributed?view=aspnetcore-5.0"},"ASP.NET Core - \u5206\u5e03\u5f0f\u7f13\u5b58")," \u7ae0\u8282\u3002")))}u.isMDXComponent=!0}}]);