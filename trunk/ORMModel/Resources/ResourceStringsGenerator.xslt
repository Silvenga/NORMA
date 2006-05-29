﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:plx="http://schemas.neumont.edu/CodeGeneration/PLiX">
	<!-- Indenting is useful for debugging the transform, but a waste of memory at generation time -->
	<xsl:output method="xml" encoding="utf-8" indent="no"/>
	<xsl:template match="ResourceStrings">
		<plx:root xmlns:plx="http://schemas.neumont.edu/CodeGeneration/PLiX">
			<plx:namespace name="Neumont.Tools.ORM">
				<plx:leadingInfo>
					<plx:comment>Common Public License Copyright Notice</plx:comment>
					<plx:comment>/**************************************************************************\</plx:comment>
					<plx:comment>* Neumont Object-Role Modeling Architect for Visual Studio                 *</plx:comment>
					<plx:comment>*                                                                          *</plx:comment>
					<plx:comment>* Copyright © Neumont University. All rights reserved.                     *</plx:comment>
					<plx:comment>*                                                                          *</plx:comment>
					<plx:comment>* The use and distribution terms for this software are covered by the      *</plx:comment>
					<plx:comment>* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *</plx:comment>
					<plx:comment>* can be found in the file CPL.txt at the root of this distribution.       *</plx:comment>
					<plx:comment>* By using this software in any fashion, you are agreeing to be bound by   *</plx:comment>
					<plx:comment>* the terms of this license.                                               *</plx:comment>
					<plx:comment>*                                                                          *</plx:comment>
					<plx:comment>* You must not remove this notice, or any other, from this software.       *</plx:comment>
					<plx:comment>\**************************************************************************/</plx:comment>
				</plx:leadingInfo>
				<plx:class name="ResourceStrings" partial="true" visibility="internal">
					<plx:leadingInfo>
						<plx:pragma type="region" data="ResourceStrings class"/>
						<plx:docComment>
							<summary>A helper class to insulate the rest of the code from direct resource manipulation.</summary>
						</plx:docComment>
					</plx:leadingInfo>
					<plx:trailingInfo>
						<plx:pragma type="closeRegion" data="ResourceStrings class"/>
					</plx:trailingInfo>
					<!--<xsl:for-each select="ResourceString">
						<plx:field const="true" visibility="private" dataTypeName=".string" name="{@name}_Id">
							<plx:initialize>
								<plx:string><xsl:value-of select="@resourceName"/></plx:string>
							</plx:initialize>
						</plx:field>
					</xsl:for-each>-->
					<xsl:for-each select="ResourceString">
						<plx:property name="{@name}" modifier="static" visibility="public">
							<xsl:if test="comment">
								<plx:leadingInfo>
									<plx:docComment>
										<xsl:for-each select="comment">
											<xsl:element name="{@type}">
												<xsl:value-of select="."/>
											</xsl:element>
										</xsl:for-each>
									</plx:docComment>
								</plx:leadingInfo>
							</xsl:if>
							<plx:returns dataTypeName=".string"/>
							<plx:get>
								<plx:return>
									<plx:callStatic name="GetString" dataTypeName="ResourceStrings">
										<plx:passParam type="in">
											<plx:callStatic name="{@model}" dataTypeName="ResourceManagers" type="field"/>
										</plx:passParam>
										<plx:passParam type="in">
											<plx:string>
												<xsl:value-of select="@resourceName"/>
											</plx:string>
											<!--<plx:callStatic name="{@name}_Id" dataTypeName="ResourceStrings" type="field"/>-->
										</plx:passParam>
									</plx:callStatic>
								</plx:return>
							</plx:get>
						</plx:property>
					</xsl:for-each>
				</plx:class>
			</plx:namespace>
		</plx:root>
	</xsl:template>
</xsl:stylesheet>