/*
 * Copyright 2013-2019 Automatak, LLC
 *
 * Licensed to Green Energy Corp (www.greenenergycorp.com) and Automatak
 * LLC (www.automatak.com) under one or more contributor license agreements. 
 * See the NOTICE file distributed with this work for additional information
 * regarding copyright ownership. Green Energy Corp and Automatak LLC license
 * this file to you under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may obtain
 * a copy of the License at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.automatak.render.dnp3.objects.generators

import com.automatak.render.dnp3.objects._

import com.automatak.render._
import com.automatak.render.cpp._

object GroupVariationHeaderRenderer extends ModelRenderer[GroupVariation]{

  def render(gv: GroupVariation)(implicit i: Indentation): Iterator[String] = comment(gv.fullDesc) ++ gv.declaration

}

object GroupVariationImplRenderer extends ModelRenderer[GroupVariation]{

  def render(gv: GroupVariation)(implicit i: Indentation): Iterator[String] = gv.implLines

}