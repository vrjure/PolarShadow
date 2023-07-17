# PolarShadow

一个可以自定义源的视频资源搜索工具。

## 原理

利用jsonpath/xpath将获取到的内容转换成合适的json结构。

## UI

MAUI Blazor + Ant Design Blazor（待定）

## 网站配置

- `sites` 属性定义所用可用的网站，是一个数组。
- 每个site的`name`为必填项,用于标识一个网站。它应是唯一的。
- `requests` 包含网站支持的请求,它是一个object。每个请求用 property-value 表示
  - `property`：请求名称
  - `value`：请求对象，包含请求和响应处理

~~~json
{
  "sites":[
    {
      "name":"...",
      "domain": "...",
      "requests": {
          "requestName":{
          }
      }
    }
  ]
}
~~~
